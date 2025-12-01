using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Domain.Common.Enums;
using Domain.Enums;

namespace CitySimulatorWPF.Converters
{
    /// <summary>
    /// Конвертер типа рельефа <see cref="TerrainType"/> в кисть (<see cref="Brush"/>)
    /// для раскраски тайлов на карте.
    /// </summary>
    /// <remarks>
    /// Используется в XAML как ресурс:
    /// <code>
    /// &lt;conv:TerrainTypeToBrushConverter x:Key="TerrainTypeToBrushConverter" /&gt;
    /// ...
    /// &lt;Border Background="{Binding TerrainType,
    ///     Converter={StaticResource TerrainTypeToBrushConverter}}" /&gt;
    /// </code>
    /// </remarks>
    public class TerrainTypeToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует значение типа <see cref="TerrainType"/> в <see cref="Brush"/>.
        /// </summary>
        /// <param name="value">Значение рельефа (ожидается <see cref="TerrainType"/>).</param>
        /// <param name="targetType">Тип целевого значения (игнорируется).</param>
        /// <param name="parameter">Дополнительный параметр (не используется).</param>
        /// <param name="culture">Информация о культуре (не используется).</param>
        /// <returns>Кисть для закраски тайла.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TerrainType terrainType)
            {
                return terrainType switch
                {
                    TerrainType.Water => Brushes.Blue,        // вода
                    TerrainType.Plain => Brushes.Green,       // равнина
                    TerrainType.Meadow => Brushes.LightGreen,  // луга
                    TerrainType.Forest => Brushes.DarkGreen,   // лес
                    TerrainType.Mountain => Brushes.DarkGray,    // горы
                    _ => Brushes.Transparent
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