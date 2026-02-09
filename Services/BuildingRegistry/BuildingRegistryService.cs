using Domain.Common.Base;
using Domain.Map;
using Services.PlaceBuilding;
using System.ComponentModel.DataAnnotations;

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

        public IEnumerable<Position> GetAccessibleNeighborTiles(MapObject obj, MapModel map)
        {
            var (placement, found) = _placementRepository.TryGetPlacement(obj);
            if (!found) yield break;

            var area = placement!.Value.Area;
            var origin = placement.Value.Position;

            for (int x = origin.X - 1; x <= origin.X + area.Width; x++)
            {
                for (int y = origin.Y - 1; y <= origin.Y + area.Height; y++)
                {
                    // тайлы вокруг здания
                    if (x == origin.X - 1 || x == origin.X + area.Width ||
                        y == origin.Y - 1 || y == origin.Y + area.Height)
                    {
                        var pos = new Position(x, y);
                        if (map.IsPositionInBounds(pos) && map[pos].MapObject == null)
                            yield return pos;
                    }
                }
            }
        } 
    }
}
