using NandC.Engine.Models;
using NandC.Engine.Simulator;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NaC.Engine.Models
{
    public class GameBootstrapper
    {
        protected readonly Player human;
        protected readonly SmartRobot smartBot;

        public GameBootstrapper(Action<Action> dispatcher = null, Game game = null)
        {
            smartBot = new SmartRobot(1);
            var dumbBot = new RandomRobot(2);

            human = new Human(3);

            Game = game ?? Game.Create(smartBot, dumbBot, human);

            GameMatrix = new ObservableCollection<GridReferenceViewModel>(Game.Board.GridReferences.Select(r => new GridReferenceViewModel(r)));
            Messages = new ObservableCollection<string>();

            if (dispatcher == null) dispatcher = new Action<Action>((x) => x());

            Game.Board.Updated += (s, e) =>
            {
                dispatcher.Invoke(() =>
                {
                    UpdateReference(e.ContextData, e.Player);
                });
            };

            Game.Board.BoardReset += (s, e) =>
            {
                dispatcher.Invoke(() =>
                {
                    foreach (var rv in GameMatrix)
                    {
                        rv.Player = null;
                    }
                });
            };

            Initialise();
        }

        public void TrainRobot()
        {
            smartBot.Learn(50000);
            smartBot.Save();
        }

        public void Start()
        {
            smartBot.AutoSave = true;
            smartBot.Load();
            human.Active = true;
            smartBot.Active = true;
            smartBot.Play();
        }

        public Game Game { get; private set; }

        public ObservableCollection<GridReferenceViewModel> GameMatrix { get; private set; }

        public ObservableCollection<string> Messages { get; private set; }

        public bool SubmitGo(Point gridRef)
        {
            try
            {
                if (human.Active)
                    Game.Board.SubmitGo(human, gridRef);
                return true;
            }
            catch (Exception ex)
            {
                Messages.Add(ex.Message);
                return false;
            }
        }

        protected void UpdateReference(Point point, Player player)
        {
            var rv = GameMatrix.Single(r => r.GridReference.Equals(point));

            rv.Player = player;
        }

        protected virtual void Initialise()
        {
        }
    }
}