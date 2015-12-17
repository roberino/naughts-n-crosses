using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NaughtsAndCrosses.Simulator
{
    public class GameViewModel
    {
        private readonly Player human;
        private readonly SmartRobot smartBot;

        public GameViewModel(Grid gameMatrix, Game game = null)
        {
            smartBot = new SmartRobot(1);
            var dumbBot = new RandomRobot(2);

            human = new Human(3);
            
            Game = game ?? Game.Create(smartBot, dumbBot, human);

            var dispatcher = gameMatrix.Dispatcher;

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
                    foreach(var rv in GameMatrix)
                    {
                        rv.Player = null;
                    }
                });
            };

            GameMatrix = new ObservableCollection<GridReferenceViewModel>(Game.Board.GridReferences.Select(r => new GridReferenceViewModel(r)));
            Messages = new ObservableCollection<string>();

            SetupGrid(gameMatrix);
        }

        public void Start()
        {
            //smartBot.Learn(50000);
            //smartBot.Save();
            smartBot.AutoSave = true;
            smartBot.Load();
            human.Active = true;
            smartBot.Play();
        }

        public Game Game { get; private set; }

        public ObservableCollection<GridReferenceViewModel> GameMatrix { get; private set; }

        public ObservableCollection<string> Messages { get; private set; }

        private void UpdateReference(Point point, Player player)
        {
            var rv = GameMatrix.Single(r => r.GridReference.Equals(point));

            rv.Player = player;
        }

        private void SubmitGo(Point gridRef)
        {
            try
            {
                if(human.Active)
                    Game.Board.SubmitGo(human, gridRef);
            }
            catch(Exception ex)
            {
                Messages.Add(ex.ToString());
            }
        }

        private void SetupGrid(Grid gridView)
        {
            foreach(var item in GameMatrix)
            {
                var border = new Border()
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    Background = new SolidColorBrush(Colors.White)
                };

                border.MouseLeftButtonDown += (s, e) =>
                {
                    SubmitGo(item.GridReference);
                };

                var text = new TextBlock()
                {
                    FontSize = 64,
                    Padding = new Thickness(10)
                };

                text.MouseLeftButtonDown += (s, e) =>
                {
                    SubmitGo(item.GridReference);
                };

                var symBinding = new Binding("Player.Symbol");
                var colBinding = new Binding("Player.Colour");
                symBinding.Source = item;
                colBinding.Source = item;
                text.SetBinding(TextBlock.TextProperty, symBinding);
                text.SetBinding(TextBlock.ForegroundProperty, colBinding);

                gridView.Children.Add(border);
                gridView.Children.Add(text);

                Grid.SetRow(border, item.GridReference.Y - 1);
                Grid.SetColumn(border, item.GridReference.X - 1);
                Grid.SetRow(text, item.GridReference.Y - 1);
                Grid.SetColumn(text, item.GridReference.X - 1);
            }
        }
    }
}