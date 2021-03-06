﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureBan.Domain
{
    public class Game
    {
        private readonly IBoard _board;
        private readonly IList<Player> _players = new List<Player>();
        private readonly HashSet<string> _playersThatMadeMove = new HashSet<string>();
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
            return _board.OpenTickets.ToList();
        }

        public Ticket StartProgressOnTicket(Ticket openTicket, Player player)
        {
            if (_players.All(x => x.Name != player.Name))
                throw new InvalidOperationException($"player {player.Name} not in game");
            if (_playersThatMadeMove.Contains(player.Name))
                throw new InvalidOperationException($"player {player.Name} already made his move");

            _board.AssignTicket(openTicket, player.Name);
            _board.MoveTicketForward(openTicket);

            _playersThatMadeMove.Add(player.Name);

            return openTicket;
        }

        public Ticket MoveTicketForward(Ticket ticket, Player player)
        {
            if (ticket.AssigneeName != player.Name)
                throw new InvalidOperationException($"Тикет {ticket.Name} назначен другому игроку ({ticket.AssigneeName})");
            if (_playersThatMadeMove.Contains(player.Name))
                throw new InvalidOperationException($"player {player.Name} already made his move");

            _board.MoveTicketForward(ticket);

            _playersThatMadeMove.Add(player.Name);

            return ticket;
        }

        public Ticket BlockTicketAndGetNew(Ticket ticket, Player player)
        {
            if (ticket.AssigneeName != player.Name)
                throw new InvalidOperationException($"Тикет {ticket.Name} назначен другому игроку ({ticket.AssigneeName})");

            _board.BlockTicket(ticket);

            var assignedTicket = _board.OpenTickets.First();
            _board.AssignTicket(assignedTicket, player.Name);

            return assignedTicket;
        }

        public Ticket UnblockTicket(Ticket ticket, Player player)
        {
            if (ticket.AssigneeName != player.Name)
                throw new InvalidOperationException($"Тикет {ticket.Name} назначен другому игроку ({ticket.AssigneeName})");

            _board.UnblockTicket(ticket);
            return _board.GetTicketByName(ticket.Name);
        }
    }
}
