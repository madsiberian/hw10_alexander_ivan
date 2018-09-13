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
            return new Board();
        }

        public Board AsWritten(string definition)
        {
            var tickets = new List<Ticket>();
            var maxTickets = new Dictionary<Stage, int>();

            var allLines = definition.Replace(" ", "").Split('\n');
            var lines = allLines.Skip(2);
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

            var stageHeaders = allLines[1].Split('|').Skip(1).Take(4).ToList();
            for (int i = 0; i < stageHeaders.Count; ++i)
            {
                var maxTicketCount = GetMaxTicketsInStage(stageHeaders[i]);
                if (maxTicketCount is null) continue;

                maxTickets.Add((Stage)i, maxTicketCount.Value);
            }



            var fakeTicketService = new Mock<ITicketService>();
            fakeTicketService.Setup(m => m.CreateTicket()).Returns(Create.Ticket().Please());
            return new Board(tickets, maxTickets);
        }

        private int? GetMaxTicketsInStage(string stageHeader)
        {
            var regex = new Regex(@"(?<stageName>\w+)(<=(?<maxTicketCount>\d+))?");
            var match = regex.Match(stageHeader);
            if (!match.Success)
                throw new ArgumentException($"{stageHeader} не распознано как описание заголовка stage");

            return match.Groups["maxTicketCount"].Success ? int.Parse(match.Groups["maxTicketCount"].Value) : (int?) null;
        }

        private Ticket ParseTicketDef(string ticketDef)
        {
            var regex = new Regex(@"\[(?<ticketName>\w+)(>(?<playerName>\w+)(?<block>\!B)?)?\]");
            var match = regex.Match(ticketDef);
            if (!match.Success)
                throw new ArgumentException($"{ticketDef} не распознано как тикет");

            var ticket = new Ticket();
            ticket.Name = match.Groups["ticketName"].Value;
            
            if (match.Groups["playerName"].Success)
            {
                ticket.AssigneeName = match.Groups["playerName"].Value;
            }

            if (match.Groups["block"].Success)
            {
                ticket.IsBlocked = true;
            }

            return ticket;
        }
    }
}