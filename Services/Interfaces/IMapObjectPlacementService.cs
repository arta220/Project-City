using Domain.Base;
using Domain.Map;

namespace Services.Interfaces
{
    public interface IMapObjectPlacementService
    {
        public bool CanPlace(MapModel map, MapObject mapObject, Area area);
        public bool TryPlace(MapModel map, MapObject mapObject, Area area);
        public void RemoveBuilding(MapModel map, Area area);

    }
}
