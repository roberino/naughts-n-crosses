using NaC.Engine.Models;
using NandC.Engine.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NandC.UI.Models
{
    public class GameViewModel : GameBootstrapper
    {
        public GameViewModel(Grid gameMatrix, Game game = null) : base(gameMatrix.Dispatcher.Invoke, game)
        {
            SetupGrid(gameMatrix);
        }

        private void SetupGrid(Grid gridView)
        {
            foreach(var item in GameMatrix)
            {
                var border = new Border()
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new System.Windows.Thickness(1),
                    Background = new SolidColorBrush(Colors.White)
                };

                border.MouseLeftButtonDown += (s, e) =>
                {
                    SubmitGo(item.GridReference);
                };

                var text = new TextBlock()
                {
                    FontSize = 64,
                    Padding = new System.Windows.Thickness(5),
                    TextAlignment = System.Windows.TextAlignment.Center
                };

                text.MouseLeftButtonDown += (s, e) =>
                {
                    SubmitGo(item.GridReference);
                };

                var symBinding = new Binding("Player.Symbol");
                var colBinding = new Binding("Player.Colour");

                symBinding.Source = item;
                colBinding.Source = item;

                colBinding.Converter = new StringToSolidColourBrushConverter();

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