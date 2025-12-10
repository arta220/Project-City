using Domain.Map;
using Domain.Common.Time;
using Services.EntityMovement.PathFind;
using Domain.Common.Base.MovingEntities;

namespace Services.EntityMovement.Service
{
    public class EntityMovementService : IEntityMovementService
    {
        private readonly IPathFinder _pathFinder;
        private readonly INavigationProfile _defaultNavigationProfile;
        
        public EntityMovementService(IPathFinder pathFinder, INavigationProfile defaultNavigationProfile)
        {
            _pathFinder = pathFinder;
            _defaultNavigationProfile = defaultNavigationProfile;
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
            
            // Используем NavigationProfile из сущности, если он есть, иначе используем дефолтный
            var profile = entity.NavigationProfile ?? _defaultNavigationProfile;
            
            var path = _pathFinder.FindPath(entity.Position, target, profile)?.ToList();
            if (path == null || path.Count == 0)
                return;

            entity.CurrentPath.Clear();
            foreach (var p in path)
                entity.CurrentPath.Enqueue(p);
        }

    }
}
