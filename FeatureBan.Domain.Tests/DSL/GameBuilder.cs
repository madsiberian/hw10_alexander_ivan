using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Moq;

namespace FeatureBan.Domain.Tests.DSL
{
    public class GameBuilder
    {
        private int _maxPlayerCount = 5;
        private IEnumerable<Player> _players = new List<Player>();
        private IBoard _board;

        public GameBuilder WithBoard(IBoard board)
        {
            _board = board;
            return this;
        }

        public GameBuilder WithFakeBoard()
        {
            _board = new Mock<IBoard>().Object;
            return this;
        }

        public GameBuilder WithMaxPlayers(int maxPlayerCount)
        {
            _maxPlayerCount = maxPlayerCount;
            return this;
        }

        public GameBuilder WithPlayers(params Player[] players)
        {
            _players = players;
            return this;
        }

        public Game Please()
        {
            var game = new Game(_board, _maxPlayerCount);
            foreach (var player in _players)
            {
                game.AddPlayer(player);
            }

            return game;
        }
    }
}