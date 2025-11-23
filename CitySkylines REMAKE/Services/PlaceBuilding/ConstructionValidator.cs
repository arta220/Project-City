using CitySkylines_REMAKE.Models.Enums;
using CitySkylines_REMAKE.Models.Map;
using Core.Models.Base;

namespace CitySkylines_REMAKE.Services.PlaceBuilding
{
    public class ConstructionValidator
    {
        public bool CanBuildOnTile(TileModel tile, Building building)
            => tile.CanPlace(building);
    }

}
