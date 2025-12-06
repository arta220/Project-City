using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Domain.Map;
using Services.Citizens.Movement;

namespace Services.CitizenSimulation.Tasks;

public class MoveToPositionTask : ICitizenTask
{
    private readonly Position _target;
    private bool _pathSet = false;
    private readonly ICitizenMovementService _movement;

    public bool IsCompleted { get; private set; }

    public MoveToPositionTask(
        Position target,
        ICitizenMovementService movement)
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
        }

        _movement.PlayMovement(citizen, time);

        if (citizen.Position == _target)
        {
            IsCompleted = true;
        }
    }
}
