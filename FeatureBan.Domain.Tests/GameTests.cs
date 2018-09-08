using System;
using Xunit;

namespace FeatureBan.Domain.Tests
{
    public class GameTests
    {
        [Fact]
        public void AddPlayer_ThenCountOfPlayersShouldIncrement()
        {
            var game = new Game();
            var player = new Player("foo");
            var countOfPlayersInGame = game.CountOfPlayers;

            game.AddPlayer(player);

            Assert.Equal(countOfPlayersInGame + 1, game.CountOfPlayers);
        }

        [Fact]
        public void AddPlayer_ThrowsInvalidOperationException_WhenPlayerIsAlredyInGame()
        {
            var game = new Game();
            var player = new Player("foo");
            game.AddPlayer(player);

            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(player));
        }
    }
}
