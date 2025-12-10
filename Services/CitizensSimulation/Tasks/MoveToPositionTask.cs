using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Domain.Map;
using Services.EntityMovement.Service;

namespace Services.Citizens.Tasks
{
    /// <summary>
    /// Простейшая таска движения гражданина в указанную точку.
    /// </summary>
    public class MoveToPositionTask : ICitizenTask
    {
        private readonly Position _target;
        private readonly IEntityMovementService _movement;
        private bool _pathSet;

        public bool IsCompleted { get; private set; }

        public MoveToPositionTask(Position target, IEntityMovementService movement)
        {
            _target = target;
            _movement = movement;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            if (!_pathSet)
            {
                _movement.SetTarget(citizen, _target);
                _pathSet = true;

                if (citizen.CurrentPath.Count == 0)
                {
                    IsCompleted = true;
                    return;
                }
            }

            _movement.PlayMovement(citizen, time);

            if (citizen.Position == citizen.TargetPosition)
                IsCompleted = true;
        }
    }
}

