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
        public void AddPlayer_ThrowsInvalidOperationException_WhenPlayerIsAlreadyInGame()
        {
            var game = new Game();
            var player = new Player("foo");
            game.AddPlayer(player);

            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(player));
        }

        [Fact]
        public void AddPlayer_ThrowsInvalidOperationException_WhenMaxPlayerCountExceeded()
        {
            var game = new Game(maxPlayerCount: 3);
            var player1 = new Player("a");
            var player2 = new Player("b");
            var player3 = new Player("c");
            var player4 = new Player("d");
            game.AddPlayer(player1);
            game.AddPlayer(player2);
            game.AddPlayer(player3);

            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(player4));
        }
    }
}
