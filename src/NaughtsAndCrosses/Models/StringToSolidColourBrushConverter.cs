using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NandC.UI.Models
{
    public class StringToSolidColourBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch (value.ToString().ToLower())
                {
                    case "red":
                        return new SolidColorBrush(Colors.Red);
                    case "blue":
                        return new SolidColorBrush(Colors.Blue);
                    case "green":
                        return new SolidColorBrush(Colors.Green);
                }
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
