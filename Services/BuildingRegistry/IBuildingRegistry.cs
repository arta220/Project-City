using Domain.Base;
using Domain.Map;

namespace Services.BuildingRegistry
{
    public interface IBuildingRegistry
    {
        IEnumerable<MapObject> GetAllBuildings();
        (Placement? placement, bool found) TryGetPlacement(MapObject building);
    }

}
