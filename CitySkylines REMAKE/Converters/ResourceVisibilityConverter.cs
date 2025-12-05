using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CitySimulatorWPF.Converters
{
    /// <summary>
    /// Комбинирует HasResource (локальный флаг) и ShowResources (глобальный флаг)
    /// в Visibility для иконки ресурса.
    /// </summary>
    public class ResourceVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return Visibility.Collapsed;

            bool hasResource = values[0] is bool b1 && b1;
            bool showResources = values[1] is bool b2 && b2;

            return (hasResource && showResources)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
