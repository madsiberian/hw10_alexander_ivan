using System.Collections.Generic;

namespace FeatureBan.Domain.Tests.DSL
{
    public class GameBuilder
    {
        private int _maxPlayerCount = 5;
        private IEnumerable<Player> _players = new List<Player>();

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
            var game = new Game(_maxPlayerCount);
            foreach (var player in _players)
            {
                game.AddPlayer(player);
            }

            return game;
        }
    }
}