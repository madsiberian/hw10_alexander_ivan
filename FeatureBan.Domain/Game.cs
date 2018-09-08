using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureBan.Domain
{
    public class Game
    {
        private readonly IList<Player> _players = new List<Player>();

        public int CountOfPlayers => _players.Count;

        public void AddPlayer(Player player)
        {
            if (_players.Any(x => x.Id == player.Id))
                throw new InvalidOperationException();

            _players.Add(player);
        }
    }
}
