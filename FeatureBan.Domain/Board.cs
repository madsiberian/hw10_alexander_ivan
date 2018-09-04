using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureBan.Domain
{
    public class Board
    {
        public virtual Ticket GetOpenTicket()
        {
            return new Ticket();
        }

        public void MoveTicketForward(Ticket ticket)
        {
            if (!ticket.IsAssigned)
                throw new InvalidOperationException();
        }

        public Ticket GetTicketByName(string name)
        {
            return new Ticket();
        }
    }
}
