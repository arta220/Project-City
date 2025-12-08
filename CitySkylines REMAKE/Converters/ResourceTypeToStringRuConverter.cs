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
    /// Конвертер типа ресурса в строку на русском.
    /// </summary>
    public class ResourceTypeToStringRuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not NaturalResourceType resourceType)
                return "Неизвестно";

            var field = typeof(NaturalResourceType).GetField(resourceType.ToString());
            if (field == null)
                return resourceType.ToString();

            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? resourceType.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
