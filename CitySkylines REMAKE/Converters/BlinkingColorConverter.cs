using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;


// Smirnov
namespace CitySimulatorWPF.Converters
{
    public class BlinkingColorConverter : IValueConverter
    {
        private static bool _blinkState;
        private static DispatcherTimer _timer;

        static BlinkingColorConverter()
        {
            // Таймер для переключения состояния мигания TODO: надо проверить вообще
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.7) };
            _timer.Tick += (s, e) =>
            {
                _blinkState = !_blinkState;
            };
            _timer.Start();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isBlinking && isBlinking)
            {
                // Мигаем)))
                return _blinkState ? Brushes.Red : Brushes.DarkRed;
            }

            // Обычный серый цвет для зданий
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}