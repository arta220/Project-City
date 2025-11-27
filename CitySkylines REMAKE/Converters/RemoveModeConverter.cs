using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


// Smirnov*
namespace CitySimulatorWPF.Converters
{
    public class RemoveModeConverter : IValueConverter
    {
        public static RemoveModeConverter Instance { get; } = new RemoveModeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isActive && isActive) ? "❌ Отменить удаление" : "🗑️ Удалить";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RemoveModeBackgroundConverter : IValueConverter
    {
        public static RemoveModeBackgroundConverter Instance { get; } = new RemoveModeBackgroundConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isActive && isActive) ? "#FF6666" : "#FF4444";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
