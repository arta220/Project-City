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
    /// Конвертер типа ресурса в строку на русском.
    /// </summary>
    public class ResourceTypeToStringRuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not NaturalResourceType type)
                return "Нет ресурса";

            return type switch
            {
                NaturalResourceType.None => "Нет ресурса",
                NaturalResourceType.Iron => "Железо",
                NaturalResourceType.Copper => "Медь",
                NaturalResourceType.Oil => "Нефть",
                NaturalResourceType.Gas => "Газ",
                NaturalResourceType.Wood => "Дерево",
                _ => "Неизвестно"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
