using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Map;

namespace Domain.Common.Base
{
    /// <summary>
    /// Базовый класс для всех объектов, которые размещаются на карте.
    /// </summary>
    /// <remarks>
    /// Содержит информацию о занимаемой площади объекта.
    /// Может быть унаследован для создания зданий, дорог, парков и других объектов.
    /// </remarks>
    public abstract class MapObject : ObservableObject
    {
        /// <summary>
        /// Площадь, занимаемая объектом на карте.
        /// </summary>
        public Area Area { get; }

        /// <summary>
        /// Создаёт объект с указанной площадью.
        /// </summary>
        /// <param name="area">Площадь объекта (ширина и высота в клетках карты).</param>
        /// <remarks>
        /// Для объектов произвольного размера, указывайте Area при конструировании.
        /// </remarks>
        protected MapObject(Area area)
        {
            Area = area;
        }
    }
}