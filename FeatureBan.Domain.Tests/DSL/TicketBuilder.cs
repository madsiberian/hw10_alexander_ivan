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
            _ticket.IsAssigned = true;
            return this;
        }

        public Ticket Please()
        {
            return _ticket;
        }

        public TicketBuilder Blocked()
        {
            _ticket.IsBlocked = true;
            return this;
        }
    }
}