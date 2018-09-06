using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
