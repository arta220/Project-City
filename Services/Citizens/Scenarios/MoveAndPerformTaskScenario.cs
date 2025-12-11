using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.Citizens.Tasks.Move;
using Services.EntityMovement.Service;
using Services.Time;

public abstract class MoveAndPerformTaskScenario : ICitizenScenario // Задача "придти куда-то и что-то сделать". Наследуются конкретные действия
{
    protected readonly IEntityMovementService _movement;
    protected readonly IBuildingRegistry _registry;

    protected MoveAndPerformTaskScenario(IEntityMovementService movement, IBuildingRegistry registry)
    {
        _movement = movement;
        _registry = registry;
    }

    public abstract bool CanRun(Citizen citizen, ISimulationTimeService time);

    protected abstract ICitizenTask CreateMainTask(Citizen citizen);

    public virtual void BuildTasks(Citizen citizen)
    {
        if (citizen.WorkPlace == null) return;

        // Сначала идем к зданию
        citizen.Tasks.Enqueue(new MoveToBuildingTask(citizen.WorkPlace, _movement, _registry));
        // Потом выполняем специфическую задачу
        citizen.Tasks.Enqueue(CreateMainTask(citizen));
    }
}
