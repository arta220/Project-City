using Domain.Base;
using Domain.Map;

namespace Services.PlaceBuilding
{
    public class ConstructionValidator
    {
        public bool CanBuildOnTile(TileModel tile, Building building)
            => tile.CanPlace(building);
    }

}
