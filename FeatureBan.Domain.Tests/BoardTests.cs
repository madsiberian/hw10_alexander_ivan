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
            var board = Create.Board().Please();
            var ticket = Create.Ticket().Assigned().OnStage(Stage.Dev).Please();

            board.MoveTicketForward(ticket);
            var TestTicket = board.GetTicketByName(ticket.Name);

            Assert.Equal(ticket.Name, TestTicket.Name);
            Assert.Equal(Stage.Test, TestTicket.Stage);
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageFromTestToDone_WhenTicketIsTest()
        {
            var board = Create.Board().Please();
            var ticket = Create.Ticket().Assigned().OnStage(Stage.Test).Please();

            board.MoveTicketForward(ticket);
            var doneTicket = board.GetTicketByName(ticket.Name);

            Assert.Equal(ticket.Name, doneTicket.Name);
            Assert.Equal(Stage.Done, doneTicket.Stage);
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsDone()
        {
            var board = Create.Board().Please();
            var ticket = Create.Ticket().Assigned().OnStage(Stage.Done).Please();

            Assert.Throws<InvalidOperationException>(() => board.MoveTicketForward(ticket));
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
