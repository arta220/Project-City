using Domain.Base;
using Services.Interfaces;
using Domain.Map;

namespace Services.PlaceBuilding
{
    public class MapObjectPlacementService : IMapObjectPlacementService
    {
        private readonly ConstructionValidator _validator;

        public MapObjectPlacementService(ConstructionValidator validator)
        {
            _validator = validator;
        }

        public bool CanPlace(MapModel map, MapObject mapObject, Area area)
        {
            if (!map.IsAreaInBounds(area))
                return false;

            foreach (var position in area.GetAllPositions())
            {
                TileModel tile = map[position];
                if (!_validator.CanBuildOnTile(tile, mapObject))
                    return false;
            }
            return true;
        }

        public bool TryPlace(MapModel map, MapObject mapObject, Area area)
        {
            if (!CanPlace(map, mapObject, area))
                return false;

            return map.TrySetMapObject(mapObject, area);
        }

        public void RemoveBuilding(MapModel map, Area area)
        {
            throw new NotImplementedException();
        }
    }
}

