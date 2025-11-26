using Domain.Base;
using Domain.Map;

namespace Services.PlaceBuilding
{
    public class PlacementRepository
    {
        private readonly Dictionary<MapObject, Placement> _placements = new();

        public void Register(MapObject obj, Placement placement)
        {
            _placements[obj] = placement;
        }

        public Placement GetPlacement(MapObject obj)
        {
            if (!_placements.TryGetValue(obj, out var placement))
                throw new InvalidOperationException("Объект не размещён на карте.");
            return placement;
        }

        public void Remove(MapObject obj)
        {
            _placements.Remove(obj);
        }

        public IEnumerable<MapObject> GetAll() => _placements.Keys;
    }

}
