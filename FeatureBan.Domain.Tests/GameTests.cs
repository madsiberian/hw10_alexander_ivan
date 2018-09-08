using AutoFixture;
using FeatureBan.Domain.Tests.DSL;
using FluentAssertions;
using System;
using System.Linq;
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
        public void StartProgressOnTicket_AssignsAndMovesTicket()
        {
            var player = Fixture.Create<Player>();
            var game = Create.Game().WithPlayers(player).Please();
            var openTicket = game.GetOpenTickets().First();

            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            Assert.Equal(openTicket.Name, ticketInWork.Name);
            Assert.Equal(Stage.WIP1, ticketInWork.Stage);
            Assert.Equal(ticketInWork.Assignee, player.Name);
        }

        [Fact]
        public void StartProgressOnTicket_ThrowsInvalidOperationException_WhenTicketIsNotOpen()
        {
            var player = Fixture.Create<Player>();
            var game = Create.Game().WithPlayers(player).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            Assert.Throws<InvalidOperationException>(() => game.StartProgressOnTicket(ticketInWork, player));
        }

        [Fact]
        public void StartProgressOnTicket_ThrowsInvalidOperationException_WhenPlayerNotInGame()
        {
            var player = Fixture.Create<Player>();
            var game = Create.Game().Please();
            var openTicket = game.GetOpenTickets().First();

            Assert.Throws<InvalidOperationException>(() => game.StartProgressOnTicket(openTicket, player));
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageToWip2_WhenTicketStageIsWip1()
        {
            var player = Fixture.Create<Player>();
            var game = Create.Game().WithPlayers(player).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            var wip2Ticket = game.MoveTicketForward(ticketInWork, player);

            wip2Ticket.Stage.Should().Be(Stage.WIP2);
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageToDone_WhenTicketStageIsWip2()
        {
            var player = Fixture.Create<Player>();
            var game = Create.Game().WithPlayers(player).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);
            var wip2Ticket = game.MoveTicketForward(ticketInWork, player);
            
            var doneTicket = game.MoveTicketForward(wip2Ticket, player);

            doneTicket.Stage.Should().Be(Stage.Done);
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsAssignedToOtherPlayer()
        {
            var players = Fixture.CreateMany<Player>().Take(2).ToArray();
            var game = Create.Game().WithPlayers(players).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, players.First());

            Assert.Throws<InvalidOperationException>(() => game.MoveTicketForward(ticketInWork, players.Last()));
        }
    }
}
