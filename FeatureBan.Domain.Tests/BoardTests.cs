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

            board.MoveTicketForward(openTicket);
            var wipTicket = board.GetTicketByName(openTicket.Name);

            Assert.Equal(openTicket.Name, wipTicket.Name);
            Assert.Equal(Stage.WIP1, wipTicket.Stage);
        }
    }
}
