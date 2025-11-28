using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Domain.Enums;
using Domain.Infrastructure;

namespace CitySimulatorWPF.Converters
{
    public class PathToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Path path)
            {
                return path.Type switch
                {
                    PathType.Pedestrian => Brushes.LightGray,
                    PathType.Bicycle => Brushes.Orange,
                    _ => Brushes.Transparent
                };
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}