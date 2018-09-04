using System;
using Moq;
using Xunit;

namespace FeatureBan.Domain.Tests
{
    public class BoardTests
    {
        [Fact]
        public void MoveTicketForward_ChangesTicketStageFromOpenToWip1_WhenTicketIsOpen()
        {
            var board = new Mock<Board>();
            board.Setup(x => x.GetOpenTicket()).Returns(new Ticket { IsAssigned = true });
            var openTicket = board.Object.GetOpenTicket();

            board.Object.MoveTicketForward(openTicket);
            var wipTicket = board.Object.GetTicketByName(openTicket.Name);

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
    }
}
