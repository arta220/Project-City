using Domain.Base;
using Domain.Map;

namespace Services.Interfaces
{
    public interface IBuildingPlacementService
    {
        public bool CanPlaceBuilding(MapModel map, Building building, Area area);
        public bool TryPlaceBuilding(MapModel map, Building building, Area area);
        public void RemoveBuilding(MapModel map, Area area);

    }
}
