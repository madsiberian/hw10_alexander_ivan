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
        public void AssignTicket_SetsIsAssignedToTrue()
        {
            var board = new Board();
            var unassignedTicket = board.GetOpenTicket();

            board.AssignTicket(unassignedTicket);
            var assignedTicket = board.GetTicketByName(unassignedTicket.Name);

            Assert.True(assignedTicket.IsAssigned);
            Assert.Equal(unassignedTicket.Name, assignedTicket.Name);
        }
    }
}
