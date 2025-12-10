using Domain.Base;
using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Domain.Map;
using Services.EntityMovement.Service;

namespace Services.Citizens.Tasks
{
    /// <summary>
    /// Таска "прогулка по парку": идёт по нескольким случайным клеткам внутри парка.
    /// </summary>
    public class WalkInParkTask : ICitizenTask
    {
        private readonly Park _park;
        private readonly Placement _placement;
        private readonly IEntityMovementService _movement;
        private readonly int _stepsToWalk;
        private readonly Random _random = new();

        private int _stepsCompleted;
        private bool _pathSet;

        public bool IsCompleted { get; private set; }

        public WalkInParkTask(Park park, Placement placement, IEntityMovementService movement, int stepsToWalk = 4)
        {
            _park = park;
            _placement = placement;
            _movement = movement;
            _stepsToWalk = Math.Max(1, stepsToWalk);
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            if (citizen.State != Domain.Citizens.States.CitizenState.RelaxingInPark)
                citizen.State = Domain.Citizens.States.CitizenState.RelaxingInPark;

            if (!_pathSet)
            {
                var target = PickRandomPositionInPark();
                _movement.SetTarget(citizen, target);
                _pathSet = true;

                if (citizen.CurrentPath.Count == 0)
                {
                    Complete(citizen);
                    return;
                }
            }

            _movement.PlayMovement(citizen, time);

            if (citizen.Position == citizen.TargetPosition)
            {
                _stepsCompleted++;
                if (_stepsCompleted >= _stepsToWalk)
                {
                    Complete(citizen);
                    return;
                }

                _pathSet = false;
            }
        }

        private Position PickRandomPositionInPark()
        {
            var positions = _placement.GetAllPositions().ToList();
            return positions[_random.Next(positions.Count)];
        }

        private void Complete(Citizen citizen)
        {
            IsCompleted = true;
            citizen.State = Domain.Citizens.States.CitizenState.Idle;
        }
    }
}

