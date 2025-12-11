using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Base;
using Domain.Common.Enums;

namespace Domain.Map
{
    /// <summary>
    /// Представляет отдельный тайл на карте.
    /// Каждый тайл содержит информацию о типе местности, высоте, расположении и объекте, если он размещён.
    /// </summary>
    public class TileModel : ObservableObject
    {
        /// <summary>
        /// Позиция тайла на карте.
        /// Используется для вычисления соседних тайлов и маршрутов граждан.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Тип местности тайла.
        /// </summary>
        public TerrainType Terrain { get; set; }

        private MapObject _mapObject;

        /// <summary>
        /// Размещённый на тайле объект (здание, дорога и т.п.).
        /// Устанавливается через SetProperty для поддержки уведомлений MVVM.
        /// </summary>
        public MapObject MapObject
        {
            get => _mapObject;
            set => SetProperty(ref _mapObject, value);
        }

        /// <summary>
        /// Высота тайла.
        /// Может использоваться для расчёта рельефа или визуальных эффектов.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Проверяет, можно ли разместить объект на этом тайле.
        /// Возвращает true, если тайл пуст.
        /// </summary>
        /// <param name="mapObject">Объект для размещения.</param>
        /// <returns>True, если размещение возможно.</returns>
        public bool CanPlace(MapObject mapObject)
        {
            return MapObject == null &&
                   Terrain != TerrainType.Water &&
                   Terrain != TerrainType.Mountain;
        }

        /// <summary>
        /// Тип природных ресурсов которые имеются у клетки
        /// </summary>
        public NaturalResourceType ResourceType { get; set; } = NaturalResourceType.None;

        /// <summary>
        /// Количество ресурсов имеющихся у клетки
        /// </summary>
        public float ResourceAmount { get; set; } = 0;
    }
}
