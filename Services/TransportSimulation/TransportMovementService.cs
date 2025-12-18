using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.EntityMovement.Service;
using Services.EntityMovement.PathFind;
using Services.EntityMovement.Profile;
using Domain.Base;

namespace Services.TransportSimulation
{
    public class TransportMovementService
    {
        private readonly IEntityMovementService _entityMovementService;
        private readonly IPathFinder _pathFinder;
        private readonly TransportProfile _transportProfile;
        private readonly MapModel _mapModel;
        private readonly Random _random = new();

        public TransportMovementService(IEntityMovementService entityMovementService, IPathFinder pathFinder, TransportProfile transportProfile, MapModel mapModel)
        {
            _entityMovementService = entityMovementService;
            _pathFinder = pathFinder;
            _transportProfile = transportProfile;
            _mapModel = mapModel;
        }

        public void UpdateTransport(Transport transport, SimulationTime time)
        {
            // If transport currently has a path, advance one step
            if (transport.CurrentPath.Count > 0)
            {
                _entityMovementService.PlayMovement(transport, time);
                return;
            }

            // Choose next target: any adjacent road tile
            var current = transport.Position;
            var neighbors = current.GetNeighbors();
            var roadNeighbors = new List<Position>();

            foreach (var n in neighbors)
            {
                if (!_mapModel.IsPositionInBounds(n)) continue;
                var tile = _mapModel[n];
                if (tile.MapObject is Road)
                    roadNeighbors.Add(n);
            }

            if (roadNeighbors.Count == 0)
            {
                // try to find any road on map within some radius (simple fallback)
                // collect up to 20 random road tiles
                var allRoads = new List<Position>();
                for (int x = 0; x < _mapModel.Width; x++)
                for (int y = 0; y < _mapModel.Height; y++)
                {
                    var pos = new Position(x, y);
                    if (_mapModel[pos].MapObject is Road) allRoads.Add(pos);
                }

                if (allRoads.Count == 0) return;

                // pick random road tile different from current
                var target = allRoads[_random.Next(allRoads.Count)];
                if (target.Equals(current)) return;

                var path = _pathFinder.FindPath(current, target, _transportProfile)?.ToList();
                if (path == null || path.Count == 0) return;

                // enqueue path (skip current position if present as first)
                transport.CurrentPath.Clear();
                foreach (var p in path)
                    transport.CurrentPath.Enqueue(p);
                return;
            }

            // pick random neighbor road as target
            var next = roadNeighbors[_random.Next(roadNeighbors.Count)];

            // compute path with A*
            var result = _pathFinder.FindPath(current, next, _transportProfile)?.ToList();
            if (result == null || result.Count == 0) return;

            transport.CurrentPath.Clear();
            foreach (var p in result)
                transport.CurrentPath.Enqueue(p);
        }

        public void SetTarget(Transport transport, Position target)
        {
            var path = _pathFinder.FindPath(transport.Position, target, _transportProfile)?.ToList();
            if (path == null || path.Count == 0) return;
            transport.CurrentPath.Clear();
            foreach (var p in path)
                transport.CurrentPath.Enqueue(p);
        }
    }
}
