using NandC.Engine.Models;
using System;
using System.Timers;
using System.Windows.Input;

namespace NandC.Engine.Simulator
{
    public abstract class Robot : Player, ICommand
    {
        protected readonly RandomSelector rand;
        private readonly Timer timer;
        protected Game game;

        public event EventHandler CanExecuteChanged;

        public Robot(int id, string name) : base(id, name)
        {
            rand = new RandomSelector();

            timer = new Timer(2000);

            timer.Elapsed += (s, e) =>
            {
                if (!Active) return;

                lock (game)
                {
                    if (!game.Board.IsComplete)
                    {
                        var p = game.Board.LastGo();

                        if (p == null || p.Id != Id) SubmitGo();
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);

                        game.Reset();

                        // timer.Stop();
                    }
                }
            };
        }

        public override void Join(Game game)
        {
            base.Join(game);
            this.game = game;
        }

        public override void Play()
        {
            base.Play();

            if (Active)
                timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public abstract void SubmitGo();

        public bool CanExecute(object parameter)
        {
            return game != null;
        }

        public void Execute(object parameter)
        {
            if (Active) Play();
            else Stop();
        }
    }
}
