using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Domain.Enums;

namespace CitySimulatorWPF.Converters
{
    public class TerrainTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TerrainType terrainType)
            {
                return terrainType switch
                {
                    TerrainType.Water => Brushes.Blue,
                    TerrainType.Plain => Brushes.Green,
                    TerrainType.Mountain => Brushes.Gray,
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
