using Domain.Base;
using Domain.Map;

namespace Services.BuildingRegistry
{
    public interface IBuildingRegistry
    {
        IEnumerable<T> GetBuildings<T>();
        (Placement? placement, bool found) TryGetPlacement<T>(T building) where T : Building;
    }

}
