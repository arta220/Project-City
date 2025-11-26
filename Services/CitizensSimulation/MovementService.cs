using Services.Interfaces;
using Domain.Citizens;
using Domain.Map;
using Services.PathFind;

namespace Services.CitizensSimulation
{
    public class MovementService : ICitizenMovementService
    {
        private readonly IPathFinder _pathFinder;

        public MovementService(IPathFinder pathfinder)
        {
            _pathFinder = pathfinder;
        }
        public void Move(Citizen citizen, Position position, int tick)
        {
            if (citizen.CurrentPath.Count == 0 || citizen.TargetPosition != position)
            {
                citizen.TargetPosition = position;
                citizen.CurrentPath = _pathFinder.FindPath(citizen.Position, citizen.TargetPosition);
            }
            if (citizen.CurrentPath.Count > 0)
            {
                citizen.Position = citizen.CurrentPath.Dequeue();
            }
        }

    }
}
