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

        private float _resourceAmount;

        /// <summary>
        /// Текущее количество ресурса, имеющегося у клетки.
        /// Для корректного обновления UI изменяется через SetProperty.
        /// </summary>
        public float ResourceAmount
        {
            get => _resourceAmount;
            set => SetProperty(ref _resourceAmount, value);
        }

        /// <summary>
        /// Максимальное (изначальное) количество ресурса на клетке.
        /// Используется для последующего восстановления ресурса (например, дерева в лесу).
        /// </summary>
        public float MaxResourceAmount { get; set; }

        /// <summary>
        /// Тик симуляции, на котором ресурс был полностью исчерпан.
        /// Null, если ресурс ещё ни разу не исчерпывался.
        /// </summary>
        public int? DepletedTick { get; set; }

        /// <summary>
        /// Задержка восстановления ресурса в тиках после полного истощения.
        /// По истечении этого времени ресурс начинает постепенно восполняться.
        /// </summary>
        public int RegenDelayTicks { get; set; } = 200;

        /// <summary>
        /// Признак того, что ресурс на клетке исчерпан (количество &lt;= 0, при наличии типа ресурса).
        /// </summary>
        public bool IsResourceDepleted =>
            ResourceType != NaturalResourceType.None &&
            ResourceAmount <= 0;
    }
}
