using Services.Interfaces;
using Domain.Map;
using Domain.Common.Base;
using Domain.Buildings;

namespace Services.PlaceBuilding
{
    /// <summary>
    /// Сервис размещения объектов на карте.
    /// </summary>
    public class MapObjectPlacementService : IMapObjectPlacementService
    {
        private readonly ConstructionValidator _validator;

        public MapObjectPlacementService(ConstructionValidator validator)
        {
            _validator = validator;
        }

        public bool CanPlace(MapModel map, MapObject mapObject, Placement area)
        {
            if (!map.IsAreaInBounds(area))
                return false;

            foreach (var position in area.GetAllPositions())
            {
                TileModel tile = map[position];

                if (!_validator.CanBuildOnTile(tile, mapObject))
                    return false;
            }

            if (mapObject is Seaport && !IsNearWater(map, area))
                return false;

            return true;
        }

        public bool TryPlace(MapModel map, MapObject mapObject, Placement area)
        {
            if (!CanPlace(map, mapObject, area))
                return false;

            return map.TrySetMapObject(mapObject, area);
        }

        public bool TryRemove(MapModel map, Placement area)
        {
            return map.TryRemoveMapObject(area);
        }

        private bool IsNearWater(MapModel map, Placement area)
        {
            foreach (var position in area.GetAllPositions())
            {
                // Слева
                if (IsWater(map, position.X - 1, position.Y))
                    return true;

                // Справа
                if (IsWater(map, position.X + 1, position.Y))
                    return true;

                // Сверху
                if (IsWater(map, position.X, position.Y - 1))
                    return true;

                // Снизу
                if (IsWater(map, position.X, position.Y + 1))
                    return true;
            }

            return false;
        }

        private bool IsWater(MapModel map, int x, int y)
        {
            if (!map.IsPositionInBounds(new Position(x, y)))
                return false;

            return map[x, y].Terrain == Domain.Common.Enums.TerrainType.Water;
        }
    }
}