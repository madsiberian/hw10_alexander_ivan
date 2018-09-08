using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureBan.Domain
{
    public class Game
    {
        private readonly IBoard _board;
        private readonly IList<Player> _players = new List<Player>();
        private readonly int _maxPlayerCount;

        public Game(IBoard board, int maxPlayerCount = 5)
        {
            _board = board;
            _maxPlayerCount = maxPlayerCount;
        }

        public int CountOfPlayers => _players.Count;

        public void AddPlayer(Player player)
        {
            if (_players.Any(x => x.Name == player.Name))
                throw new InvalidOperationException();
            if (_players.Count + 1 > _maxPlayerCount)
                throw new InvalidOperationException();

            _players.Add(player);
        }

        public IReadOnlyList<Ticket> GetOpenTickets()
        {
            return new List<Ticket>{new Ticket() { Name = "ticket name" }};
        }

        public Ticket StartProgressOnTicket(Ticket openTicket, Player player)
        {
            if (_players.All(x => x.Name != player.Name))
                throw new InvalidOperationException();

            _board.AssignTicket(openTicket, player.Name);
            _board.MoveTicketForward(openTicket);

            return openTicket;
        }

        public Ticket MoveTicketForward(Ticket ticket, Player player)
        {
            if (ticket.AssigneeName != player.Name)
                throw new InvalidOperationException();

            _board.MoveTicketForward(ticket);

            return ticket;
        }

        public Ticket BlockTicketAndGetNew(Ticket ticket, Player player)
        {
            _board.BlockTicket(ticket);

            var assignedTicket = _board.GetOpenTicket();
            _board.AssignTicket(assignedTicket, player.Name);

            return assignedTicket;
        }
    }
}
