using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureBan.Domain
{
    public class Board : IBoard
    {
        private readonly IDictionary<string, Ticket> _tickets = new Dictionary<string, Ticket>();
        private readonly ITicketService _ticketService;
        private readonly IReadOnlyDictionary<Stage, int> _maxTickets = new Dictionary<Stage, int>();

        public Board(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public Board(List<Ticket> tickets, IReadOnlyDictionary<Stage, int> maxTickets, ITicketService ticketService)
        {
            _ticketService = ticketService;
            _maxTickets = maxTickets;
            _tickets = tickets.ToDictionary(x => x.Name);
        }

        public IEnumerable<Ticket> OpenTickets => _tickets.Where(t => t.Value.Stage == Stage.Open).Select(kv => kv.Value);
        public IEnumerable<Ticket> TicketsInDev => _tickets.Where(t => t.Value.Stage == Stage.Dev).Select(kv => kv.Value);
        public IEnumerable<Ticket> TicketsInTest => _tickets.Where(t => t.Value.Stage == Stage.Test).Select(kv => kv.Value);
        public IEnumerable<Ticket> DoneTickets => _tickets.Where(t => t.Value.Stage == Stage.Done).Select(kv => kv.Value);

        public void MoveTicketForward(Ticket ticket)
        {
            if (ticket.AssigneeName == null)
                throw new InvalidOperationException($"Тикет {ticket.Name} назначен другому игроку ({ticket.AssigneeName})");
            if (ticket.IsBlocked)
                throw new InvalidOperationException($"Тикет {ticket.Name} блокирован");
            if (ticket.Stage == Stage.Done)
                throw new InvalidOperationException($"Нельзя двигать уже завершённый тикет {ticket.Name}");

            var nextStage = GetNextStage(ticket.Stage);
            if (_maxTickets.ContainsKey(nextStage) && _tickets.Count(t => t.Value.Stage == nextStage) >= _maxTickets[nextStage])
                throw new InvalidOperationException($"Стадия {nextStage.ToString()} уже заполнена");

            ticket.Stage = nextStage;

            _tickets[ticket.Name] = ticket;
        }

        private Stage GetNextStage(Stage ticketStage)
        {
            switch (ticketStage)
            {
                case Stage.Open:
                    return Stage.Dev;
                case Stage.Dev:
                    return Stage.Test;
                case Stage.Test:
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
            if (ticket.AssigneeName != null)
                throw new InvalidOperationException();
            
            ticket.AssigneeName = playerId;
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
