﻿using NandC.Engine.Models;
using System.Linq;

namespace NandC.Engine.Simulator
{
    public class Trainer
    {
        private readonly Game game;

        public Trainer(Game game = null)
        {
            this.game = game ?? new Game();
        }

        public GameStats Train(int iterations)
        {
            return Train(new GameStats(game.Players.First()), iterations);
        }

        public GameStats Train(GameStats data, int iterations)
        {
            var rand = new RandomSelector();

            var player1 = game.Players.First();
            var player2 = game.Players.Skip(1).First();

            player1.Active = true;
            player2.Active = true;

            foreach (var n in Enumerable.Range(1, iterations))
            {
                bool isPlayer1 = true;

                while (!game.Board.IsComplete)
                {
                    var avail = game.Board.AvailableReferences.ToList();

                    var reference = rand.Select(avail, -1);

                    if (reference == -1) break;

                    var player = isPlayer1 ? player1 : player2;

                    game.Board.SubmitGo(player, reference);

                    isPlayer1 = (!isPlayer1);
                }

                data.Register(game);

                game.Board.Reset();
            }

            return data;
        }
    }
}
