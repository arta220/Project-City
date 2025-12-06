using Domain.Common.Base;
using Domain.Map;
using Services.PathFind;

namespace Services.TransportSimulation
{
    public class TransportMovementService
    {
        private readonly IPathFinder _pathFinder;

        public TransportMovementService(IPathFinder pathFinder)
        {
            _pathFinder = pathFinder;
        }

        public void Move(MovingEntity transport, Position target)
        {
            if (transport.CurrentPath.Count == 0 || transport.TargetPosition != target)
            {
                transport.TargetPosition = target;
             //   _pathFinder.FindPath(transport.Position, transport.TargetPosition, transport.CurrentPath);
            }

            if (transport.CurrentPath.Count > 0)
            {
                transport.Position = transport.CurrentPath.Dequeue();
            }
        }
    }
}
