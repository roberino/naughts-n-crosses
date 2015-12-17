using System;
using System.Windows.Input;

namespace NaughtsAndCrosses.Simulator
{
    public class Human : Player, ICommand
    {
        private Game game;

        public Human(int id) : base(id, "Human")
        {
        }

        public override string Symbol
        {
            get
            {
                return "Sam";
            }
        }

        public override void Join(Game game)
        {
            base.Join(game);

            this.game = game;

            OnStateChange();
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return (game != null && !game.Board.IsComplete);
        }

        public void Execute(object parameter)
        {
            if(Active && parameter is int)
            {
                game.Board.SubmitGo(this, (int)parameter);
            }
        }

        private void OnStateChange()
        {
            var ev = CanExecuteChanged;

            if (ev != null) ev.Invoke(this, EventArgs.Empty);
        }
    }
}
