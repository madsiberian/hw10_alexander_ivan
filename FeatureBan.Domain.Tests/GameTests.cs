using AutoFixture;
using FeatureBan.Domain.Tests.DSL;
using FluentAssertions;
using System;
using System.Linq;
using Moq;
using Xunit;

namespace FeatureBan.Domain.Tests
{
    public class GameTests : TestBase
    {
        [Fact]
        public void AddPlayer_ThenCountOfPlayersShouldIncrement()
        {
            var game = Create.Game().WithFakeBoard().Please();
            var player = new Player("foo");
            var countOfPlayersInGame = game.CountOfPlayers;

            game.AddPlayer(player);

            Assert.Equal(countOfPlayersInGame + 1, game.CountOfPlayers);
        }

        [Fact]
        public void AddPlayer_ThrowsInvalidOperationException_WhenPlayerIsAlreadyInGame()
        {
            var game = Create.Game().WithFakeBoard().Please();
            var player = new Player("foo");
            game.AddPlayer(player);

            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(player));
        }

        [Fact]
        public void AddPlayer_ThrowsInvalidOperationException_WhenMaxPlayerCountExceeded()
        {
            var players = Fixture.CreateMany<Player>().Take(3).ToArray();
            var game = Create.Game().WithFakeBoard().WithMaxPlayers(3).WithPlayers(players).Please();

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
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();

            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            Assert.Equal(openTicket.Name, ticketInWork.Name);
            Assert.Equal(Stage.WIP1, ticketInWork.Stage);
            Assert.Equal(ticketInWork.AssigneeName, player.Name);
        }

        [Fact]
        public void StartProgressOnTicket_ThrowsInvalidOperationException_WhenTicketIsNotOpen()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
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
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            var wip2Ticket = game.MoveTicketForward(ticketInWork, player);

            wip2Ticket.Stage.Should().Be(Stage.WIP2);
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageToDone_WhenTicketStageIsWip2()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);
            var wip2Ticket = game.MoveTicketForward(ticketInWork, player);

            var doneTicket = game.MoveTicketForward(wip2Ticket, player);

            doneTicket.Stage.Should().Be(Stage.Done);
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsDone()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);
            var wip2Ticket = game.MoveTicketForward(ticketInWork, player);
            var doneTicket = game.MoveTicketForward(wip2Ticket, player);

            Assert.Throws<InvalidOperationException>(() => game.MoveTicketForward(doneTicket, player));
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsAssignedToOtherPlayer()
        {
            var players = Fixture.CreateMany<Player>().Take(2).ToArray();
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(players).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, players.First());

            Assert.Throws<InvalidOperationException>(() => game.MoveTicketForward(ticketInWork, players.Last()));
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsBlocked()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);
            game.BlockTicketAndGetNew(ticketInWork, player);

            Assert.Throws<InvalidOperationException>(() => game.MoveTicketForward(ticketInWork, player));
        }

        [Fact]
        public void BlockTicketAndGetNew_BlocksTicketAndAssignsNewTicket()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            Ticket assignedTicket = game.BlockTicketAndGetNew(ticketInWork, player);

            ticketInWork.IsBlocked.Should().BeTrue();
            assignedTicket.Stage.Should().Be(Stage.Open);
            assignedTicket.AssigneeName.Should().Be(player.Name);
            assignedTicket.IsBlocked.Should().BeFalse();
        }

        [Fact]
        public void BlockTicketAndGetNew_ThrowsInvalidOperationException_WhenTicketIsBlocked()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);
            game.BlockTicketAndGetNew(ticketInWork, player);

            Assert.Throws<InvalidOperationException>(() => game.BlockTicketAndGetNew(ticketInWork, player));
        }

        [Fact]
        public void BlockTicketAndGetNew_ThrowsInvalidOperationException_WhenTicketIsAssignedToOtherPlayer()
        {
            var players = Fixture.CreateMany<Player>().Take(2).ToArray();
            var board = Create.Board().Please();
            var game = Create.Game().WithPlayers(players).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();
            var ticketInWork = game.StartProgressOnTicket(openTicket, players.First());
            game.BlockTicketAndGetNew(ticketInWork, players.First());

            Assert.Throws<InvalidOperationException>(() => game.BlockTicketAndGetNew(ticketInWork, players.Last()));
        }
    }
}
