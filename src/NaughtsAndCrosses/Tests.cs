using NandC.Engine.Models;
using NandC.Engine.Simulator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NandC
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void CreateNewGame_BoardIsCreated()
        {
            var game = new Game();

            Assert.That(game.Board, Is.Not.Null);
            Assert.That(game.Players.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Board_SubmitGo_CurrentState()
        {
            var game = new Game();

            var player1 = game.Players.First();
            var player2 = game.Players.Last();

            game.Board.SubmitGo(player1, 1);
            game.Board.SubmitGo(player2, 2);

            var state = game.Board.CurrentState(player1);

            Assert.That(state[0], Is.GreaterThan(0));
            Assert.That(state[1], Is.GreaterThan(0));
        }

        [Test]
        public void SubmitGo_MarksComplete()
        {
            var game = new Game();

            var player1 = game.Players.First();
            var player2 = game.Players.Last();

            game.Board.SubmitGo(player1, 1);
            Assert.IsFalse(game.Board.HasWinner);
            game.Board.SubmitGo(player2, 5);
            Assert.IsFalse(game.Board.HasWinner);
            game.Board.SubmitGo(player1, 2);
            Assert.IsFalse(game.Board.HasWinner);
            game.Board.SubmitGo(player2, 7);
            Assert.IsFalse(game.Board.HasWinner);
            game.Board.SubmitGo(player1, 3);
            Assert.IsTrue(game.Board.HasWinner);
        }

        [Test]
        public void ScoreMove_ReturnsLowerScoreForLessMovedToWin()
        {
            var stats = new GameStats(new Player(1));

            stats.Register(new byte[] { 1, 3, 7, 5 }, true);
            stats.Register(new byte[] { 1, 3, 5, 7, 9 }, true);

            var score1 = stats.ScoreMove(new byte[] { 1, 3, 7 });
            var score2 = stats.ScoreMove(new byte[] { 1, 3, 5 });

            Assert.That(score2, Is.GreaterThan(score1));
        }

        [Test]
        public void ScoreMove_Returns0ForPreviousKnownOpponentWin()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);
            var game = Game.Create(p1, p2);

            var stats = new GameStats(new Player(1));

            game.Board.SubmitGo(p2, 1);
            game.Board.SubmitGo(p1, 4);
            game.Board.SubmitGo(p2, 2);
            game.Board.SubmitGo(p1, 5);
            game.Board.SubmitGo(p2, 3);

            Assert.That(game.Board.IsComplete);

            stats.Register(game);

            game.Reset();

            game.Board.SubmitGo(p2, 1);
            game.Board.SubmitGo(p1, 4);
            game.Board.SubmitGo(p2, 2);
            game.Board.SubmitGo(p1, 5);

            var p1Move = game.Board.PreviewState(8, p1, false);
            var opponentMove = game.Board.PreviewState(3, p1, true);
            var score = stats.ScoreMove(opponentMove, false);

            Assert.That(score, Is.EqualTo(0));
        }

        [Test]
        public void ScoreMove_Returns1IfWinOneMoveAway()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);
            var game = Game.Create(p1, p2);

            var stats = new GameStats(new Player(1));

            game.Board.SubmitGo(p1, 1);
            game.Board.SubmitGo(p2, 4);
            game.Board.SubmitGo(p1, 2);
            game.Board.SubmitGo(p2, 5);
            game.Board.SubmitGo(p1, 3);

            Assert.That(game.Board.IsComplete);

            stats.Register(game);

            game.Reset();

            game.Board.SubmitGo(p1, 1);
            game.Board.SubmitGo(p2, 4);
            game.Board.SubmitGo(p1, 2);
            game.Board.SubmitGo(p2, 5);

            var p1Move1 = game.Board.PreviewState(3, p1, false);
            var score1 = stats.ScoreMove(p1Move1, true);
            var p1Move2 = game.Board.PreviewState(8, p1, false);
            var score2 = stats.ScoreMove(p1Move2, true);

            Assert.That(score1, Is.EqualTo(0));
            Assert.That(!score2.HasValue);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Hasher_Hash_ReturnsPrimes(bool isOpponent)
        {
            foreach (var n in Enumerable.Range(1, 9))
            {
                var x = Hasher.Hash(n, isOpponent);
                Assert.That(Primes.IsPrime(x));
                Console.WriteLine("{0}/{1}\t{2}", n, isOpponent, x);
            }
        }

        public void Hasher_Hash_ReturnsEvenDistribution()
        {
            var data = Enumerable
                .Range(1, 9)
                .Select(n => Hasher.Hash(n, false))
                .Concat(
                    Enumerable.Range(1, 9)
                    .Select(n => Hasher.Hash(n, true)))
                .ToArray();

            var rnd = new Random(DateTime.Now.Millisecond);

            for (var i = 0; i < 10000; i++)
            {
                var items = new Queue<byte>(data.OrderBy(r => rnd.Next()));

                for (var n = 0; i < rnd.Next(8); n++)
                {

                }
            }
        }

        [TestCase(7, true)]
        [TestCase(2, true)]
        [TestCase(1, true)]
        [TestCase(23, true)]
        [TestCase(29, true)]
        [TestCase(4, false)]
        [TestCase(21, false)]
        [TestCase(200, false)]
        public void Primes_IsPrime_ReturnsValidOutput(int n, bool expectation)
        {
            var f = Primes.Factors(n).ToList();
            Assert.That(Primes.IsPrime(n) == expectation);
        }

        [Test]
        public void ScoreMove_IgnoresPlayOrder()
        {
            var stats = new GameStats(new Player(1));

            stats.Register(new byte[] { 1, 3, 7, 5 }, true);
            stats.Register(new byte[] { 1, 3, 5, 7, 13 }, true);
            stats.Register(new byte[] { 3, 1, 5, 9, 11 }, true);

            var score1 = stats.ScoreMove(new byte[] { 1, 3, 7 });
            var score2 = stats.ScoreMove(new byte[] { 1, 3, 5 });

            Assert.That(score1 == 1);
            Assert.That(score2 == 2);
        }

        [Test]
        public void Hasher_Hash_HasMinimalClashes()
        {
            for (var i = 0; i < 10; i++)
            {
                Hasher.Hash(i, true);
            }
        }

        [Test]
        public void Trainer_Train100Games()
        {
            var trainer = new Trainer();

            var data = trainer.Train(100);

            Assert.That(data, Is.Not.Null);
        }

        [TestCase(1, 1, 1)]
        [TestCase(3, 1, 3)]
        [TestCase(5, 2, 2)]
        [TestCase(8, 3, 2)]
        public void ReferenceToPoint_ReturnsCorrectCoords(int r, int x, int y)
        {
            //1 = 1,1
            //3 = 1,3
            //5 = 2,2
            //8 = 2,3

            var board = new Board();

            var pos = board.GridReference(r);

            Assert.That(pos.X, Is.EqualTo(x));
            Assert.That(pos.Y, Is.EqualTo(y));
        }

        [TestCase(50)]
        [TestCase(100)]
        [TestCase(200)]
        public void Robot_PlayGame_WinsOverRandomSelection(int numberOfGames)
        {            
            var smartRobot = new SmartRobot(1);
            var dumbRobot = new RandomRobot(2);

            var game = Game.Create(smartRobot, dumbRobot);

            smartRobot.Learn(500);

            for (int i = 0; i < numberOfGames; i++)
            {
                smartRobot.Learn(50);

                game.Board.Reset();

                while (!game.Board.IsComplete)
                {
                    smartRobot.SubmitGo();

                    if (!game.Board.IsComplete)
                    {
                        dumbRobot.SubmitGo();
                    }
                }
            }

            Console.WriteLine("Robot wins {0} / random {1}", smartRobot.Wins, numberOfGames - smartRobot.Wins);

            Assert.That(smartRobot.Wins, Is.GreaterThan(numberOfGames / 2));
        }
    }
}
