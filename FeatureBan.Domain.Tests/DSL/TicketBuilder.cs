using AutoFixture;

namespace FeatureBan.Domain.Tests.DSL
{
    public class TicketBuilder
    {
        private Ticket _ticket = new Ticket();

        public TicketBuilder OnStage(Stage stage)
        {
            _ticket.Stage = stage;
            return this;
        }

        public TicketBuilder Assigned()
        {
            return this;
        }

        public TicketBuilder Blocked()
        {
            _ticket.IsBlocked = true;
            return this;
        }

        public Ticket Please()
        {
            _ticket.Name = new Fixture().Create<string>();
            return _ticket;
        }
    }
}