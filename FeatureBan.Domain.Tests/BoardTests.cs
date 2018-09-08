using FeatureBan.Domain.Tests.DSL;
using System;
using Xunit;

namespace FeatureBan.Domain.Tests
{
    public class BoardTests
    {
        [Fact]
        public void MoveTicketForward_ChangesTicketStageFromOpenToWip1_WhenTicketIsOpen()
        {
            var board = new Board();
            var openTicket = board.GetOpenTicket();
            board.AssignTicket(openTicket);

            board.MoveTicketForward(openTicket);
            var wipTicket = board.GetTicketByName(openTicket.Name);

            Assert.Equal(openTicket.Name, wipTicket.Name);
            Assert.Equal(Stage.WIP1, wipTicket.Stage);
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsNotAssigned()
        {
            var board = new Board();
            var unassignedTicket = board.GetOpenTicket();

            Assert.Throws<InvalidOperationException>(() => board.MoveTicketForward(unassignedTicket));
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsBlocked()
        {
            var board = new Board();
            var ticket = Create.Ticket().OnStage(Stage.WIP1).Assigned().Blocked().Please();

            Assert.Throws<InvalidOperationException>(() => board.MoveTicketForward(ticket));
        }

        [Fact]
        public void AssignTicket_SetsIsAssignedToTrue()
        {
            var board = new Board();
            var unassignedTicket = board.GetOpenTicket();

            board.AssignTicket(unassignedTicket);
            var assignedTicket = board.GetTicketByName(unassignedTicket.Name);

            Assert.True(assignedTicket.IsAssigned);
            Assert.Equal(unassignedTicket.Name, assignedTicket.Name);
        }

        [Fact]
        public void AssignTicket_ThrowsInvalidOperationException_WhenTicketIsAlreadyAssigned()
        {
            var board = new Board();
            var ticket = Create.Ticket().Assigned().Please();

            Assert.Throws<InvalidOperationException>(() => board.AssignTicket(ticket));
        }

        [Fact]
        public void BlockTicket_SetsIsBlockedToTrue()
        {
            var board = new Board();
            var ticket = Create.Ticket().OnStage(Stage.WIP1).Assigned().Please();

            board.BlockTicket(ticket);

            Assert.True(ticket.IsBlocked);
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_WhenTicketIsOpen()
        {
            var board = new Board();
            var openTicket = board.GetOpenTicket();

            Assert.Throws<InvalidOperationException>(() => board.BlockTicket(openTicket));
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_WhenTicketIsDone()
        {
            var board = new Board();
            var ticket = Create.Ticket().OnStage(Stage.Done).Assigned().Please();

            Assert.Throws<InvalidOperationException>(() => board.BlockTicket(ticket));
        }

        [Fact]
        public void BlockTicket_ThrowsInvalidOperationException_IfWeTryBlockAgain()
        {
            var board = new Board();
            var ticket = board.GetOpenTicket();
            board.AssignTicket(ticket);
            board.MoveTicketForward(ticket);

            board.BlockTicket(ticket);

            Assert.Throws<InvalidOperationException>(() => board.BlockTicket(ticket));
        }

        [Fact]
        public void UnblockTicket_SetsIsBlockedToFalse()
        {
            var board = new Board();
            var ticket = Create.Ticket().OnStage(Stage.WIP1).Assigned().Blocked().Please();

            board.UnblockTicket(ticket);

            Assert.False(ticket.IsBlocked);
        }

        [Fact]
        public void UnblockTicket_ThrowsInvalidOperationException_WhenTicketIsNotBlocked()
        {
            var board = new Board();
            var ticket = Create.Ticket().OnStage(Stage.WIP1).Assigned().Please();

            Assert.Throws<InvalidOperationException>(() => board.UnblockTicket(ticket));
        }
    }
}
