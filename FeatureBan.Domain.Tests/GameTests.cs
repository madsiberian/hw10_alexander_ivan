using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FeatureBan.Domain.Tests
{
    public class GameTests
    {
        [Fact]
        public void AddPlayer_ThenCountOfPlayersShouldIncrement()
        {
            var game = new Game();
            var player = new Player();
            var countOfPlayersInGame = game.CountOfPlayers;

            game.AddPlayer(player);

            Assert.Equal(countOfPlayersInGame + 1, game.CountOfPlayers);
        }
    }
}
