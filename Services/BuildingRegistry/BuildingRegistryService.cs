using Domain.Common.Base;
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

        public BuildingRegistryService(PlacementRepository placementRepository)
        {
            _placementRepository = placementRepository;
        }

        public IEnumerable<T> GetBuildings<T>() => _placementRepository.GetAll().OfType<T>();

        public (Placement? placement, bool found) TryGetPlacement(MapObject building) => _placementRepository.TryGetPlacement(building);
    }
}
