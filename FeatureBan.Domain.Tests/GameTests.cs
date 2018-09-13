using AutoFixture;
using FeatureBan.Domain.Tests.DSL;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var board = Create.Board().AsWritten(@"
                | Open        | Dev                | Test | Done                |
                |[OpenTicket1]|[DevTicket > Player]|      |                     |
                |[OpenTicket2]|                    |      |[DoneTicket > Player]|");
            var game = Create.Game().WithBoard(board).Please();
            var expectedOpenTickets = new List<Ticket>
            {
                new Ticket {Name = "OpenTicket1", Stage = Stage.Open},
                new Ticket {Name = "OpenTicket2", Stage = Stage.Open}
            };

            var tickets = game.GetOpenTickets();

            tickets.Should().BeEquivalentTo(expectedOpenTickets);
        }

        [Fact]
        public void StartProgressOnTicket_AssignsAndMovesTicket()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().AsWritten(@"
                | Open   | Dev | Test | Done |
                |[Ticket]|     |      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = board.OpenTickets.Single();

            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            Assert.Equal(openTicket.Name, ticketInWork.Name);
            Assert.Equal(Stage.Dev, ticketInWork.Stage);
            Assert.Equal(ticketInWork.AssigneeName, player.Name);
        }

        [Fact]
        public void StartProgressOnTicket_ThrowsInvalidOperationException_WhenTicketIsNotOpen()
        {
            var player = new Player("Player");
            var board = Create.Board().AsWritten(@"
                | Open | Dev             | Test | Done |
                |      |[Ticket > Player]|      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.Single();

            Assert.Throws<InvalidOperationException>(() => game.StartProgressOnTicket(ticketInWork, player));
        }

        [Fact]
        public void StartProgressOnTicket_ThrowsInvalidOperationException_WhenPlayerNotInGame()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().AsWritten(@"
                | Open   | Dev | Test | Done |
                |[Ticket]|     |      |      |");
            var game = Create.Game().WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().First();

            Assert.Throws<InvalidOperationException>(() => game.StartProgressOnTicket(openTicket, player));
        }

        [Fact]
        public void StartProgressOnTicket_ThrowsInvalidOperationException_WhenPlayerMadeHisMove()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().AsWritten(@"
                | Open        | Dev | Test | Done |
                |[FirstTicket]|     |      |      |
                |[LastTicket] |     |      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTickets = game.GetOpenTickets().Take(2).ToArray();
            game.StartProgressOnTicket(openTickets.First(), player);

            Assert.Throws<InvalidOperationException>(() => game.StartProgressOnTicket(openTickets.Last(), player));
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenPlayerMadeHisMove()
        {
            var player = new Player("Player");
            var board = Create.Board().AsWritten(@"
                | Open   | Dev | Test | Done |
                |[Ticket]|     |      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var openTicket = game.GetOpenTickets().Single();
            var ticketInWork = game.StartProgressOnTicket(openTicket, player);

            Assert.Throws<InvalidOperationException>(() => game.MoveTicketForward(ticketInWork, player));
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageToTest_WhenTicketStageIsDev()
        {
            var player = new Player("Player");
            var board = Create.Board().AsWritten(@"
                | Open | Dev             | Test | Done |
                |      |[Ticket > Player]|      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.Single();

            var testTicket = game.MoveTicketForward(ticketInWork, player);

            testTicket.Stage.Should().Be(Stage.Test);
        }

        [Fact]
        public void MoveTicketForward_ChangesTicketStageToDone_WhenTicketStageIsTest()
        {
            var player = new Player("Player");
            var board = Create.Board().AsWritten(@"
                | Open | Dev | Test              | Done |
                |      |     | [Ticket > Player] |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var testTicket = board.TicketsInTest.Single();

            var doneTicket = game.MoveTicketForward(testTicket, player);

            doneTicket.Stage.Should().Be(Stage.Done);
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsDone()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().AsWritten(@"
                | Open | Dev | Test | Done              |
                |      |     |      | [Ticket > Player] |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var doneTicket = board.DoneTickets.Single();

            Assert.Throws<InvalidOperationException>(() => game.MoveTicketForward(doneTicket, player));
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsAssignedToOtherPlayer()
        {
            var players = new[] {new Player("Player"), new Player("OtherPlayer")};
            var board = Create.Board().AsWritten(@"
                | Open | Dev                    | Test | Done |
                |      | [Ticket > OtherPlayer] |      |      |");
            var game = Create.Game().WithPlayers(players).WithBoard(board).Please();
            var firstPlayer = players.First();
            var ticketInWork = board.TicketsInDev.Single();

            Assert.Throws<InvalidOperationException>(() => game.MoveTicketForward(ticketInWork, firstPlayer));
        }

        [Fact]
        public void MoveTicketForward_ThrowsInvalidOperationException_WhenTicketIsBlocked()
        {
            var player = new Player("Player");
            var board = Create.Board().AsWritten(@"
                | Open | Dev                | Test | Done |
                |      |[Ticket > Player !B]|      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.Single();

            Assert.Throws<InvalidOperationException>(() => game.MoveTicketForward(ticketInWork, player));
        }

        [Fact]
        public void BlockTicketAndGetNew_BlocksTicketAndAssignsNewTicket()
        {
            var player = new Player("Player");
            var board = Create.Board().AsWritten(@"
                | Open         | Dev             | Test | Done |
                | [OpenTicket] |[Ticket > Player]|      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.Single();

            var assignedTicket = game.BlockTicketAndGetNew(ticketInWork, player);

            ticketInWork.IsBlocked.Should().BeTrue();
            assignedTicket.Stage.Should().Be(Stage.Open);
            assignedTicket.AssigneeName.Should().Be(player.Name);
            assignedTicket.IsBlocked.Should().BeFalse();
        }

        [Fact]
        public void BlockTicketAndGetNew_ThrowsInvalidOperationException_WhenTicketIsBlocked()
        {
            var player = new Player("Player");
            var board = Create.Board().AsWritten(@"
                | Open         | Dev                | Test | Done |
                | [OpenTicket] |[Ticket > Player !B]|      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.Single();

            Assert.Throws<InvalidOperationException>(() => game.BlockTicketAndGetNew(ticketInWork, player));
        }

        [Fact]
        public void BlockTicketAndGetNew_ThrowsInvalidOperationException_WhenTicketIsAssignedToOtherPlayer()
        {
            var players = new[] {new Player("FirstPlayer"), new Player("OtherPlayer")};
            var firstPlayer = players.First();
            var board = Create.Board().AsWritten(@"
                | Open         | Dev                  | Test | Done |
                | [OpenTicket] |[Ticket > OtherPlayer]|      |      |");
            var game = Create.Game().WithPlayers(players).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.Single();

            Assert.Throws<InvalidOperationException>(() => game.BlockTicketAndGetNew(ticketInWork, firstPlayer));
        }

        [Fact]
        public void UnblockTicket()
        {
            var player = new Player("Player");
            var board = Create.Board().AsWritten(@"
                | Open | Dev                | Test | Done |
                |      |[Ticket > Player !B]|      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.Single();

            var unblockedTicket = game.UnblockTicket(ticketInWork, player);

            unblockedTicket.Should().BeEquivalentTo(ticketInWork, o => o.Excluding(t => t.IsBlocked));
            unblockedTicket.IsBlocked.Should().BeFalse();
        }

        [Fact]
        public void UnblockTicket_ThrowInvalidOperation_WhenTicketInUnblocked()
        {
            var player = Fixture.Create<Player>();
            var board = Create.Board().AsWritten(@"
                | Open | Dev             | Test | Done |
                |      |[Ticket > Player]|      |      |");
            var game = Create.Game().WithPlayers(player).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.Single();

            Assert.Throws<InvalidOperationException>(() => game.UnblockTicket(ticketInWork, player));
        }

        [Fact]
        public void UnblockTicket_ThrowInvalidOperation_WhenTicketIsAssignedToOtherPlayer()
        {
            var players = new[] { new Player("FirstPlayer"), new Player("OtherPlayer") };
            var firstPlayer = players.First();
            var board = Create.Board().AsWritten(@"
                | Open | Dev                     | Test | Done |
                |      |[Ticket > OtherPlayer !B]|      |      |");
            var game = Create.Game().WithPlayers(players).WithBoard(board).Please();
            var ticketInWork = board.TicketsInDev.First();

            Assert.Throws<InvalidOperationException>(() => game.UnblockTicket(ticketInWork, firstPlayer));
        }
    }
}
