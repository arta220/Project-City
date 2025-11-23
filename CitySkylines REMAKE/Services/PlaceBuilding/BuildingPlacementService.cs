using CitySkylines_REMAKE.Models.Map;
using CitySkylines_REMAKE.Services.Interfaces;
using Core.Models.Base;
namespace CitySkylines_REMAKE.Services.PlaceBuilding
{
    public class BuildingPlacementService : IBuildingPlacementService
    {
        private readonly ConstructionValidator _validator;

        public BuildingPlacementService(ConstructionValidator validator)
        {
            _validator = validator;
        }

        public bool CanPlaceBuilding(MapModel map, Building building, Area area)
        {
            if (!map.IsAreaInBounds(area))
                return false;

            foreach (var position in area.GetAllPositions())
            {
                TileModel tile = map[position];
                if (!_validator.CanBuildOnTile(tile, building))
                    return false;
            }
            return true;
        }

        public bool TryPlaceBuilding(MapModel map, Building building, Area area)
        {
            if (!CanPlaceBuilding(map, building, area))
                return false;

            return map.TrySetBuilding(building, area);
        }

        public void RemoveBuilding(MapModel map, Area area)
        {
            throw new NotImplementedException();
        }
    }
}

