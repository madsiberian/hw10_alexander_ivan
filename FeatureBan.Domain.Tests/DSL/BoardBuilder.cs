using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        public Board AsWritten(string definition)
        {
            var tickets = new List<Ticket>();
            var lines = definition.Replace(" ", "").Split('\n').Skip(2);
            foreach (var line in lines)
            {
                var stages = line.Split('|').Skip(1).ToList();
                for (int i = 0; i < stages.Count; ++i)
                {
                    var ticketDef = stages[i];
                    if (string.IsNullOrEmpty(ticketDef))
                        continue;

                    var ticket = ParseTicketDef(ticketDef);
                    ticket.Stage = (Stage)i;

                    tickets.Add(ticket);
                }
            }
            var fakeTicketService = new Mock<ITicketService>();
            fakeTicketService.Setup(m => m.CreateTicket()).Returns(Create.Ticket().Please());
            return new Board(tickets, fakeTicketService.Object);
        }

        private Ticket ParseTicketDef(string ticketDef)
        {
            var regex = new Regex(@"\[(?<ticketName>\w+)(>(?<playerName>\w+(?<block>\!B)?)?)\]");
            var match = regex.Match(ticketDef);
            if (!match.Success)
                throw new ArgumentException($"{ticketDef} не распознано как тикет");

            var ticket = new Ticket();
            ticket.Name = match.Groups["ticketName"].Value;
            
            if (match.Groups["playerName"].Success)
            {
                ticket.AssigneeName = match.Groups["ticketName"].Value;
                ticket.IsAssigned = true;
            }

            if (match.Groups["block"].Success)
            {
                ticket.IsBlocked = true;
            }

            return ticket;
        }
    }
}