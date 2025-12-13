using Domain.Map;
using Domain.Common.Time;
using Services.EntityMovement.PathFind;
using Domain.Common.Base.MovingEntities;

namespace Services.EntityMovement.Service
{
    public class EntityMovementService : IEntityMovementService
    {
        private readonly IPathFinder _pathFinder;
        private readonly INavigationProfile _navigationProfile;
        public EntityMovementService(IPathFinder pathFinder, INavigationProfile navigationProfile)
        {
            _pathFinder = pathFinder;
            _navigationProfile = navigationProfile;
        }

        public void PlayMovement(MovingEntity entity, SimulationTime time)
        {
            if (entity.CurrentPath.Count == 0)
                return;

            entity.Position = entity.CurrentPath.Dequeue();
        }


        public void RecalculatePath(MovingEntity entity) => SetTarget(entity, entity.TargetPosition);

        public void SetTarget(MovingEntity entity, Position target)
        {
            entity.TargetPosition = target;
            var path = _pathFinder.FindPath(entity.Position, target, _navigationProfile)?.ToList();
            if (path == null || path.Count == 0)
                return;

            entity.CurrentPath.Clear();
            foreach (var p in path)
                entity.CurrentPath.Enqueue(p);
        }

    }
}
