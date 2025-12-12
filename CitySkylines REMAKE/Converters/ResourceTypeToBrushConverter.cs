using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Domain.Common.Enums;
using Domain.Enums;

namespace CitySimulatorWPF.Converters
{
    /// <summary>
    /// Конвертер типа природного ресурса <see cref="NaturalResourceType"/> 
    /// в кисть (<see cref="Brush"/>) для отображения ресурсов на карте.
    /// </summary>
    /// <remarks>
    /// Используется в XAML как ресурс для заполнения маленького кружка ресурса:
    /// <code>
    /// &lt;conv:ResourceTypeToBrushConverter x:Key="ResourceTypeToBrushConverter" /&gt;
    /// ...
    /// &lt;Ellipse
    ///     Fill="{Binding ResourceType,
    ///                    Converter={StaticResource ResourceTypeToBrushConverter}}"
    ///     Visibility="{Binding HasResource,
    ///                          Converter={StaticResource BooleanToVisibilityConverter}}"/&gt;
    /// </code>
    /// </remarks>
    public class ResourceTypeToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует значение типа <see cref="NaturalResourceType"/> в <see cref="Brush"/>.
        /// </summary>
        /// <param name="value">Значение ресурса (ожидается <see cref="NaturalResourceType"/>).</param>
        /// <param name="targetType">Тип целевого значения (игнорируется).</param>
        /// <param name="parameter">Дополнительный параметр (не используется).</param>
        /// <param name="culture">Информация о культуре (не используется).</param>
        /// <returns>
        /// Кисть для визуализации ресурса на карте, либо прозрачную кисть,
        /// если ресурс отсутствует или тип не распознан.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NaturalResourceType type)
            {
                return type switch
                {
                    NaturalResourceType.Iron => Brushes.LightGray,    // железо
                    NaturalResourceType.Coal => Brushes.DarkGray,     // уголь
                    NaturalResourceType.Copper => Brushes.Orange,       // медь
                    NaturalResourceType.Oil => Brushes.Black,        // нефть
                    NaturalResourceType.Gas => Brushes.LightSkyBlue, // газ
                    NaturalResourceType.Wood => Brushes.SaddleBrown, // дерево
                    _ => Brushes.Transparent   // нет ресурса / неизвестный тип
                };
            }

            return Brushes.Transparent;
        }

        /// <summary>
        /// Обратное преобразование не поддерживается.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}