using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FeatureBan.Domain
{
    public class Game
    {
        private readonly Board _board = new Board();
        private readonly IList<Player> _players = new List<Player>();
        private readonly int _maxPlayerCount;

        public Game(int maxPlayerCount = 5)
        {
            this._maxPlayerCount = maxPlayerCount;
        }

        public int CountOfPlayers => _players.Count;

        public void AddPlayer(Player player)
        {
            if (_players.Any(x => x.Id == player.Id))
                throw new InvalidOperationException();
            if (_players.Count + 1 > _maxPlayerCount)
                throw new InvalidOperationException();

            _players.Add(player);
        }

        public IReadOnlyList<Ticket> GetOpenTickets()
        {
            return new List<Ticket>{new Ticket()};
        }

        public Ticket StartProgressOnTicket(Ticket openTicket, Player player)
        {
            _board.AssignTicket(openTicket, player.Id);
            _board.MoveTicketForward(openTicket);

            return openTicket;
        }
    }
}
