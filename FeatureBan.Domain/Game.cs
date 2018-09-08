using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureBan.Domain
{
    public class Game
    {
        private readonly IList<Player> _players = new List<Player>();

        public int CountOfPlayers => _players.Count;

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }
    }
}
