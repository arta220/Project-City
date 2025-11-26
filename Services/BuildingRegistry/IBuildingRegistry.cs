using Domain.Base;
using Domain.Map;

namespace Services.BuildingRegistry
{
    public interface IBuildingRegistry
    {
        IEnumerable<MapObject> GetAllBuildings();
        Placement GetPlacement(MapObject building);
    }

}
