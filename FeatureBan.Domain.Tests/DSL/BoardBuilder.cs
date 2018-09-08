using Moq;

namespace FeatureBan.Domain.Tests.DSL
{
    public class BoardBuilder
    {
        public Board Please()
        {
            var fakeTicketService = new Mock<ITicketService>();
            fakeTicketService.Setup(m => m.CreateTicket()).Returns(Create.Ticket().Please());
            return new Board(fakeTicketService.Object);
        }
    }
}