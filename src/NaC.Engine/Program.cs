using NaC.Engine.Models;
using NandC.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NaC.Engine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var game = new GameBootstrapper();

            game.Game.Board.Updated += (s, e) =>
            {
                RenderGrid(game.GameMatrix);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Last move: {0} ({1},{2})", e.Player.Name, e.ContextData.X, e.ContextData.Y);
            };

            game.Game.Board.Completed += (s, e) =>
            {
                Console.WriteLine("Game over");
                if (game.Game.Board.HasWinner)
                {
                    Console.WriteLine("Winner: {0}", game.Game.Board.Winner);
                }
            };

            Console.Clear();
            Console.WriteLine("Running");

            Console.Write("Train robot? ");

            if( Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.Clear();
                Console.WriteLine("Training...");
                game.TrainRobot();
            }

            game.Start();

            var p = new RefParser();

            while (true)
            {
                var k = Console.ReadKey();

                if (k.Key == ConsoleKey.Escape) break;

                if (p.SetNext(k.KeyChar).GetValueOrDefault())
                {
                    if (!game.SubmitGo(p.Point))
                    {
                        Console.WriteLine();
                        Console.WriteLine(game.Messages.Last());
                    }

                    p.Reset();
                }
            }

            Console.WriteLine("Exiting");
        }

        private static void RenderGrid(IEnumerable<GridReferenceViewModel> view)
        {
            int lastY = -1;

            Console.Clear();

            foreach (var refx in view.OrderBy(y => y.GridReference.Y).ThenBy(x => x.GridReference.X))
            {
                if(lastY < refx.GridReference.Y)
                {
                    Console.WriteLine();
                }

                Console.Write(" ");
                Console.Write(refx.Player == null ? "-" : refx.Player.Symbol);
                Console.Write(" |");

                lastY = refx.GridReference.Y;
            }
        }

        private class RefParser
        {
            private int x = -1;
            private int y = -1;

            public Point Point { get; private set; }

            public void Reset()
            {
                x = -1;
                y = -1;
                Point = new Point();
            }

            public bool? SetNext(char key)
            {
                byte v;

                if (byte.TryParse(key.ToString(), out v) && v > 0 && v <= 3)
                {
                    if (x > -1)
                    {
                        y = v;
                        Point = new Point(x, y);
                        return true;
                    }
                    else
                    {
                        x = v;
                        return false;
                    }
                }

                return null;
            }
        }
    }
}