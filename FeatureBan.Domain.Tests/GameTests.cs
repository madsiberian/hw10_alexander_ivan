using AutoFixture;
using FeatureBan.Domain.Tests.DSL;
using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace FeatureBan.Domain.Tests
{
    public class GameTests : TestBase
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
            var players = Fixture.CreateMany<Player>().Take(3).ToArray();
            var game = Create.Game().WithMaxPlayers(3).WithPlayers(players).Please();

            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(new Player("extra player")));
        }

        [Fact]
        public void GetOpenTickets_ReturnsOpenTickets()
        {
            var game = Create.Game().Please();

            var tickets = game.GetOpenTickets();

            tickets.Should().NotBeEmpty();
            tickets.Should().NotContain(t => t.Stage != Stage.Open);
        }

        [Fact]
        public void StartProgress_AssignsAndMovesTicket()
        {
            var player = Fixture.Create<Player>();
            var game = Create.Game().WithPlayers(player).Please();
            var openTicket = game.GetOpenTickets().First();

            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            Assert.Equal(openTicket.Name, ticketInWork.Name);
            Assert.Equal(Stage.WIP1, ticketInWork.Stage);
            Assert.Equal(ticketInWork.Assignee, player.Id);
        }
    }
}
