using Domain.Citizens;
using Domain.Map;
using Services.PathFind;
using Domain.Common.Time;
namespace Services.Citizens.Movement
{
    public class MovementService : ICitizenMovementService
    {
        private readonly IPathFinder _pathFinder;
        public MovementService(IPathFinder pathfinder)
        {
            _pathFinder = pathfinder;
        }

        public void PlayMovement(Citizen citizen, SimulationTime time)
        {
            if (citizen.CurrentPath.Count == 0)
                return;

            citizen.Position = citizen.CurrentPath.Dequeue();
        }

        public void RecalculatePath(Citizen citizen) => SetTarget(citizen, citizen.TargetPosition);

        public void SetTarget(Citizen citizen, Position target)
        {
            citizen.TargetPosition = target;
            citizen.CurrentPath.Clear();

            var path = _pathFinder.FindPath(citizen.Position, target);
            if (path == null) return;

            foreach (var p in path)
                citizen.CurrentPath.Enqueue(p);
        }
    }
}
