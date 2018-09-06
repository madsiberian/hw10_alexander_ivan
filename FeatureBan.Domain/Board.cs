using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureBan.Domain
{
    public class Board
    {
        private readonly IList<Ticket> _tickets = new List<Ticket>();

        public Ticket GetOpenTicket()
        {
            var ticket = new Ticket();
            _tickets.Add(ticket);
            return ticket;
        }

        public void MoveTicketForward(Ticket ticket)
        {
            if (!ticket.IsAssigned)
                throw new InvalidOperationException();
            if (ticket.IsBlocked)
                throw new InvalidOperationException();

            ticket.Stage = GetNextStage(ticket.Stage);
        }

        private Stage GetNextStage(Stage ticketStage)
        {
            switch (ticketStage)
            {
                case Stage.Open:
                    return Stage.WIP1;
                case Stage.WIP1:
                    return Stage.WIP2;
                case Stage.WIP2:
                    return Stage.Done;
                default:
                    throw new ArgumentException();
            }
        }

        public Ticket GetTicketByName(string name)
        {
            return _tickets.FirstOrDefault(x => x.Name == name);
        }

        public void AssignTicket(Ticket ticket)
        {
            if (ticket.IsAssigned)
                throw new InvalidOperationException();

            ticket.IsAssigned = true;
        }

        public void BlockTicket(Ticket ticket)
        {
            if (ticket.IsBlocked) throw new InvalidOperationException();

            if (ticket.Stage == Stage.Open || ticket.Stage == Stage.Done)
                throw new InvalidOperationException();

            ticket.IsBlocked = true;
        }
    }
}
