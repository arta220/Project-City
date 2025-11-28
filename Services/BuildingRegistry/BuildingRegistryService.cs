using Domain.Base;
using Domain.Map;
using Services.PlaceBuilding;

namespace Services.BuildingRegistry
{
    /// <summary>
    /// Сервис для управления всеми зданиями на карте.
    /// Предоставляет методы для получения информации о размещённых зданиях и их позициях.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется другими сервисами, такими как <see cref="CitizenController"/> для определения,
    ///   где находятся дома или рабочие места жителей.
    /// - Используется в логике симуляции для проверки допустимости размещения зданий.
    /// - MapVM или другие VM могут обращаться к сервису для отображения зданий на карте.
    /// - Можно расширить, добавив фильтры по типу здания, этажности, населению и т.п.
    /// </remarks>
    public class BuildingRegistryService : IBuildingRegistry
    {
        private readonly PlacementRepository _placementRepository;

        /// <summary>
        /// Инициализирует сервис с использованием репозитория размещений.
        /// </summary>
        /// <param name="placementRepository">Репозиторий, хранящий информацию о всех размещённых объектах.</param>
        public BuildingRegistryService(PlacementRepository placementRepository)
        {
            _placementRepository = placementRepository;
        }

        /// <summary>
        /// Возвращает все здания, зарегистрированные в системе.
        /// </summary>
        /// <returns>Коллекция всех объектов MapObject (зданий) на карте.</returns>
        public IEnumerable<MapObject> GetAllBuildings() => _placementRepository.GetAll();

        /// <summary>
        /// Получает текущее размещение (позицию и площадь) указанного здания на карте.
        /// </summary>
        /// <param name="building">Здание, для которого необходимо узнать размещение.</param>
        /// <returns>Структура Placement с координатами и размером здания.</returns>
        public (Placement? placement, bool found) TryGetPlacement(MapObject building) => _placementRepository.TryGetPlacement(building);
    }
}
