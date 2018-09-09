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

            board.Invoking(b => b.MoveTicketForward(b.DoneTickets.Single()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsNotAssigned()
        {
            var board = Create.Board().AsWritten(@"
                | Open   | Dev | Test | Done |
                |[Ticket]|     |      |      |");

            board.Invoking(b => b.MoveTicketForward(b.OpenTickets.Single()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsBlocked()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev                | Test | Done |
                |      |[Ticket > Player !B]|      |      |");

            board.Invoking(b => b.MoveTicketForward(b.TicketsInDev.Single()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AssignTicket_SetsAssigneeName()
        {
            var board = Create.Board().AsWritten(@"
                | Open   | Dev | Test | Done |
                |[Ticket]|     |      |      |");

            board.AssignTicket(board.OpenTickets.Single(), "some player");

            board.OpenTickets.Single().AssigneeName.Should().Be("some player");
        }

        [Fact]
        public void AssignTicket_ThrowsInvalidOperationException_WhenTicketIsAlreadyAssigned()
        {
            var board = Create.Board().AsWritten(@"
                | Open            | Dev | Test | Done |
                |[Ticket > Player]|     |      |      |");

            board.Invoking(b => b.AssignTicket(b.OpenTickets.Single(), "OtherPlayer"))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BlockTicket_SetsIsBlockedToTrue()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev                | Test | Done |
                |      |[Ticket > Player   ]|      |      |");

            var expectedBoard = Create.Board().AsWritten(@"
                | Open | Dev                | Test | Done |
                |      |[Ticket > Player !B]|      |      |");

            board.BlockTicket(board.TicketsInDev.Single());

            board.Should().BeEquivalentTo(expectedBoard);
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_WhenTicketIsOpen()
        {
            var board = Create.Board().AsWritten(@"
                | Open            | Dev | Test | Done |
                |[Ticket > Player]|     |      |      |");

            board.Invoking(b => b.BlockTicket(b.OpenTickets.Single()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_WhenTicketIsDone()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev | Test | Done             |
                |      |     |      |[Ticket > Player] |");

            board.Invoking(b => b.BlockTicket(b.DoneTickets.Single()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_IfWeTryBlockAgain()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev                 | Test | Done |
                |      |[Ticket > Player !B] |      |      |");

            board.Invoking(b => b.BlockTicket(b.TicketsInDev.Single()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UnblockTicket_SetsIsBlockedToFalse()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev                 | Test | Done |
                |      |[Ticket > Player !B] |      |      |");
            var expectedBoard = Create.Board().AsWritten(@"
                | Open | Dev                 | Test | Done |
                |      |[Ticket > Player   ] |      |      |");

            board.UnblockTicket(board.TicketsInDev.Single());

            board.Should().BeEquivalentTo(expectedBoard);
        }

        [Fact]
        public void UnblockTicket_ThrowsInvalidOperationException_WhenTicketIsNotBlocked()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev              | Test | Done |
                |      |[Ticket > Player] |      |      |");

            board.Invoking(b => b.UnblockTicket(b.TicketsInDev.Single()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenMovingTicketFromOpenAndDevStageIsFull()
        {
            var board = Create.Board().AsWritten(@"
                | Open                 | Dev <= 1               | Test | Done |
                |[MovedTicket > Player]|[TicketOnStage > Player]|      |      |");

            board.Invoking(b => b.MoveTicketForward(b.OpenTickets.Single()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenMovingTicketFromDevAndTestStageIsFull()
        {
            var board = Create.Board().AsWritten(@"
                | Open | Dev                  | Test <= 1              | Done |
                |      |[MovedTicket > Player]|[TicketOnStage > Player]|      |");

            board.Invoking(b => b.MoveTicketForward(b.TicketsInDev.Single()))
                .Should().Throw<InvalidOperationException>();
        }
    }
}
