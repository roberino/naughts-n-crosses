using System;
using System.Linq;

namespace NaughtsAndCrosses.Simulator
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
            var rand = new Random(DateTime.Now.Millisecond);

            var player1 = game.Players.First();
            var player2 = game.Players.Skip(1).First();
            
            foreach (var n in Enumerable.Range(1, iterations))
            {
                bool isPlayer1 = true;

                while (!game.Board.IsComplete)
                {
                    var avail = game.Board.AvailableReferences.ToList();

                    if (avail.Count == 0)
                    {
                        break;
                    }

                    var reference = avail[rand.Next(avail.Count)];

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
