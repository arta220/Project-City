using Domain.Base;
using Domain.Map;

namespace Services.Interfaces
{
    public interface IMapObjectPlacementService
    {
        bool CanPlace(MapModel map, MapObject mapObject, Placement area);
        bool TryPlace(MapModel map, MapObject mapObject, Placement area);
        bool TryRemove(MapModel map, Placement area);

    }
}
