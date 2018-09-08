using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureBan.Domain
{
    public class Board : IBoard
    {
        private readonly IDictionary<string, Ticket> _tickets = new Dictionary<string, Ticket>();
        private readonly ITicketService _ticketService;

        public Board(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public Ticket GetOpenTicket()
        {
            var ticket = _ticketService.CreateTicket();
            _tickets.Add(ticket.Name, ticket);
            return ticket;
        }

        public void MoveTicketForward(Ticket ticket)
        {
            if (!ticket.IsAssigned)
                throw new InvalidOperationException();
            if (ticket.IsBlocked)
                throw new InvalidOperationException();
            if (ticket.Stage == Stage.Done)
                throw new InvalidOperationException();

            ticket.Stage = GetNextStage(ticket.Stage);
            _tickets[ticket.Name] = ticket;
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
            return _tickets.FirstOrDefault(x => x.Key == name).Value;
        }

        public void AssignTicket(Ticket ticket, string playerId)
        {
            if (ticket.IsAssigned)
                throw new InvalidOperationException();

            ticket.IsAssigned = true;
            ticket.Assignee = playerId;
        }

        public void BlockTicket(Ticket ticket)
        {
            if (ticket.IsBlocked)
                throw new InvalidOperationException();

            if (ticket.Stage == Stage.Open || ticket.Stage == Stage.Done)
                throw new InvalidOperationException();

            ticket.IsBlocked = true;
        }

        public void UnblockTicket(Ticket ticket)
        {
            if (!ticket.IsBlocked)
                throw new InvalidOperationException();

            ticket.IsBlocked = false;
        }
    }
}
