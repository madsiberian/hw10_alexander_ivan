using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureBan.Domain
{
    public class Board
    {
        public Ticket GetOpenTicket()
        {
            return new Ticket();
        }

        public void MoveTicketForward(Ticket ticket)
        {
        }

        public Ticket GetTicketByName(string name)
        {
            return new Ticket();
        }
    }
}
