using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureBan.Domain.Tests.DSL
{
    public static class Create
    {
        public static TicketBuilder Ticket()
        {
            return new TicketBuilder();
        }
    }
}
