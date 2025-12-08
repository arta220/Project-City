using Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

            var field = typeof(TerrainType).GetField(terrain.ToString());
            if (field == null)
                return terrain.ToString();

            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? terrain.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
