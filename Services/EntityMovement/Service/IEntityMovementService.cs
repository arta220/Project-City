using Domain.Map;
using Domain.Common.Time;
using Domain.Common.Base.MovingEntities;

namespace Services.EntityMovement.Service
{
    public interface IEntityMovementService
    {
        void SetTarget(MovingEntity entity, Position target);
        void PlayMovement(MovingEntity entity, SimulationTime time);
        void RecalculatePath(MovingEntity entity);
    }
}
