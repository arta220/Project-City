using Domain.Common.Base;
using Domain.Map;
using Services.BuildingRegistry;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Mocks
{
    public class FakeBuildingRegistry : IBuildingRegistry
    {
        private readonly List<MapObject> _buildings = new();
        private readonly Dictionary<MapObject, Placement> _placements = new();

        public void Add(MapObject building, Placement? placement = null)
        {
            _buildings.Add(building);
            if (placement.HasValue)
            {
                _placements[building] = placement.Value;
            }
        }

        public IEnumerable<T> GetBuildings<T>() => _buildings.OfType<T>();

        public (Placement? placement, bool found) TryGetPlacement(MapObject building)
        {
            if (_placements.TryGetValue(building, out var placement))
            {
                return (placement, true);
            }
            return (null, false);
        }

        public IEnumerable<Position> GetAccessibleNeighborTiles(MapObject obj, MapModel map)
        {
            return Enumerable.Empty<Position>();
        }
    }
}