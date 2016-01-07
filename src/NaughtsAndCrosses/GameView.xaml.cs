using NandC.UI.Models;
using System.Windows;

namespace NandC.UI
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
