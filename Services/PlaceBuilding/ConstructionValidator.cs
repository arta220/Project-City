using Domain.Base;
using Domain.Map;

namespace Services.PlaceBuilding
{
    public class ConstructionValidator
    {
        public bool CanBuildOnTile(TileModel tile, MapObject mapObject)
            => tile.CanPlace(mapObject);
    }

}
