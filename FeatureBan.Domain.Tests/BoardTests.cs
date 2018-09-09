using FeatureBan.Domain.Tests.DSL;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace FeatureBan.Domain.Tests
{
    public class BoardTests : TestBase
    {
        [Fact]
        public void OpenTickets_ReturnsOneOpenTicket_WhenBoardCreatedWithOneOpenTicket()
        {
            var board = Create.Board().AsWritten(@"
                | Open   | Dev | Test | Done |
                |[Ticket]|     |      |      |");

            Ticket expectedTicket =
                new Ticket
                {
                    Name = "Ticket",
                    Stage = Stage.Open,
                    IsBlocked = false
                };

            board.OpenTickets.Should().AllBeEquivalentTo(expectedTicket);
            board.OpenTickets.Count().Should().Be(1);
        }
        [Fact]
        public void TicketsInDev_ReturnsOneTicketInDev_WhenBoardCreatedWithOneTicketInDev()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev             | Test | Done |
                |      |[Ticket > Player]|      |      |");

            Ticket expectedTicket =
                new Ticket
                {
                    Name = "Ticket",
                    AssigneeName = "Player",
                    Stage = Stage.Dev,
                    IsBlocked = false,
                    IsAssigned = true
                };

            board.TicketsInDev.Should().AllBeEquivalentTo(expectedTicket);
            board.TicketsInDev.Count().Should().Be(1);
        }
        [Fact]
        public void TicketsInTest_ReturnsOneTicketInTest_WhenBoardCreatedWithOneTicketInTest()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev | Test            | Done |
                |      |     |[Ticket > Player]|      |");

            Ticket expectedTicket =
                new Ticket
                {
                    Name = "Ticket",
                    AssigneeName = "Player",
                    Stage = Stage.Test,
                    IsBlocked = false,
                    IsAssigned = true
                };

            board.TicketsInTest.Should().AllBeEquivalentTo(expectedTicket);
            board.TicketsInTest.Count().Should().Be(1);
        }

        [Fact]
        public void DoneTickets_ReturnsOneDoneTicket_WhenBoardCreatedWithOneDoneTicket()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev | Test | Done            |
                |      |     |      |[Ticket > Player]|");

            Ticket expectedTicket =
                new Ticket
                {
                    Name = "Ticket",
                    AssigneeName = "Player",
                    Stage = Stage.Done,
                    IsBlocked = false,
                    IsAssigned = true
                };

            board.DoneTickets.Should().AllBeEquivalentTo(expectedTicket);
            board.DoneTickets.Count().Should().Be(1);
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageFromOpenToDev_WhenTicketIsOpen()
        {
            var board = Create.Board().AsWritten(@"
                | Open            | Dev | Test | Done |
                |[Ticket > Player]|     |      |      |");

            var expectedBoard = Create.Board().AsWritten(@"
                | Open | Dev             | Test | Done |
                |      |[Ticket > Player]|      |      |");

            board.MoveTicketForward(board.OpenTickets.Single());

            board.Should().BeEquivalentTo(expectedBoard);
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageFromDevToTest_WhenTicketIsDev()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev             | Test | Done |
                |      |[Ticket > Player]|      |      |");

            var expectedBoard = Create.Board().AsWritten(@"
                | Open | Dev | Test            | Done |
                |      |     |[Ticket > Player]|      |");

            board.MoveTicketForward(board.TicketsInDev.Single());

            board.Should().BeEquivalentTo(expectedBoard);
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageFromTestToDone_WhenTicketIsTest()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev | Test             | Done |
                |      |     |[Ticket > Player] |      |");

            var expectedBoard = Create.Board().AsWritten(@"
                | Open | Dev | Test | Done             |
                |      |     |      |[Ticket > Player] |");

            board.MoveTicketForward(board.TicketsInTest.Single());

            board.Should().BeEquivalentTo(expectedBoard);
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsDone()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev | Test | Done             |
                |      |     |      |[Ticket > Player] |");

            Assert.Throws<InvalidOperationException>(() => board.MoveTicketForward(board.DoneTickets.Single()));
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsNotAssigned()
        {
            var board = Create.Board().Please();
            var unassignedTicket = board.GetOpenTicket();

            Assert.Throws<InvalidOperationException>(() => board.MoveTicketForward(unassignedTicket));
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsBlocked()
        {
            var board = Create.Board().Please();
            var ticket = Create.Ticket().OnStage(Stage.Dev).Assigned().Blocked().Please();

            Assert.Throws<InvalidOperationException>(() => board.MoveTicketForward(ticket));
        }

        [Fact]
        public void AssignTicket_SetsIsAssignedToTrue()
        {
            var board = Create.Board().Please();
            var unassignedTicket = board.GetOpenTicket();

            board.AssignTicket(unassignedTicket, "some player");
            var assignedTicket = board.GetTicketByName(unassignedTicket.Name);

            Assert.True(assignedTicket.IsAssigned);
            Assert.Equal(unassignedTicket.Name, assignedTicket.Name);
        }

        [Fact]
        public void AssignTicket_ThrowsInvalidOperationException_WhenTicketIsAlreadyAssigned()
        {
            var board = Create.Board().Please();
            var ticket = Create.Ticket().Assigned().Please();

            Assert.Throws<InvalidOperationException>(() => board.AssignTicket(ticket, "some player"));
        }

        [Fact]
        public void BlockTicket_SetsIsBlockedToTrue()
        {
            var board = Create.Board().Please();
            var ticket = Create.Ticket().OnStage(Stage.Dev).Assigned().Please();

            board.BlockTicket(ticket);

            Assert.True(ticket.IsBlocked);
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_WhenTicketIsOpen()
        {
            var board = Create.Board().Please();
            var openTicket = board.GetOpenTicket();

            Assert.Throws<InvalidOperationException>(() => board.BlockTicket(openTicket));
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_WhenTicketIsDone()
        {
            var board = Create.Board().Please();
            var ticket = Create.Ticket().OnStage(Stage.Done).Assigned().Please();

            Assert.Throws<InvalidOperationException>(() => board.BlockTicket(ticket));
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_IfWeTryBlockAgain()
        {
            var board = Create.Board().Please();
            var ticket = board.GetOpenTicket();
            board.AssignTicket(ticket, "some player");
            board.MoveTicketForward(ticket);

            board.BlockTicket(ticket);

            Assert.Throws<InvalidOperationException>(() => board.BlockTicket(ticket));
        }

        [Fact]
        public void UnblockTicket_SetsIsBlockedToFalse()
        {
            var board = Create.Board().Please();
            var ticket = Create.Ticket().OnStage(Stage.Dev).Assigned().Blocked().Please();

            board.UnblockTicket(ticket);

            Assert.False(ticket.IsBlocked);
        }

        [Fact]
        public void UnblockTicket_ThrowsInvalidOperationException_WhenTicketIsNotBlocked()
        {
            var board = Create.Board().Please();
            var ticket = Create.Ticket().OnStage(Stage.Dev).Assigned().Please();

            Assert.Throws<InvalidOperationException>(() => board.UnblockTicket(ticket));
        }
    }
}
