using NaughtsAndCrosses.Simulator;
using System.Windows;

namespace NaughtsAndCrosses
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GameView : Window
    {

        public GameView()
        {
            InitializeComponent();

            var view = new GameViewModel(GameCanvas);
            DataContext = view;

            view.Start();
        }
    }
}
