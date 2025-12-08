using Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CitySimulatorWPF.Converters
{
    /// <summary>
    /// Конвертер типа рельефа в человекочитаемую строку на русском.
    /// </summary>
    public class TerrainTypeToStringRuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TerrainType terrain)
                return "Неизвестно";

            return terrain switch
            {
                TerrainType.Water => "Вода",
                TerrainType.Plain => "Равнина",
                TerrainType.Meadow => "Луг",
                TerrainType.Forest => "Лес",
                TerrainType.Mountain => "Горы",
                _ => "Неизвестно"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
