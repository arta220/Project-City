using Domain.Common.Base;
using Domain.Map;

namespace Services.BuildingRegistry
{
    public interface IBuildingRegistry
    {
        IEnumerable<T> GetBuildings<T>();
        (Placement? placement, bool found) TryGetPlacement(MapObject building);
        IEnumerable<Position> GetAccessibleNeighborTiles(MapObject obj, MapModel map);
    }

}
