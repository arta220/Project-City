using Domain.Common.Base;
using Domain.Map;
using Services.Interfaces;
using Services.PlaceBuilding;
using System.Collections.Generic;

namespace Tests.Mocks
{
    public class FakePlacementService : IMapObjectPlacementService
    {
        private readonly List<Placement> _placedObjects = new();

        public IEnumerable<Placement> PlacedObjects => _placedObjects;

        public bool TryPlace(MapModel map, MapObject obj, Placement placement)
        {
            _placedObjects.Add(placement);
            return true;
        }

        public void RemovePlacement(Placement placement)
        {
            _placedObjects.Remove(placement);
        }

        public IEnumerable<Placement> GetPlacements(MapObject obj)
        {
            return _placedObjects;
        }

        public void Clear()
        {
            _placedObjects.Clear();
        }

        public bool CanPlace(MapModel map, MapObject mapObject, Placement area)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(MapModel map, Placement area)
        {
            throw new NotImplementedException();
        }
    }
}
