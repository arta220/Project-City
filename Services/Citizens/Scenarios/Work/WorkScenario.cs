using Domain.Citizens;
using Domain.Citizens.Tasks;
using Services.BuildingRegistry;
using Services.Citizens.Tasks.Job;
using Services.EntityMovement.Service;
using Services.Time;

namespace Services.Citizens.Scenarios.Work;
public class WorkScenario : MoveAndPerformTaskScenario
{
    public WorkScenario(IEntityMovementService movement, IBuildingRegistry registry)
        : base(movement, registry)
    {
    }

    public override bool CanRun(Citizen citizen, ISimulationTimeService time)
    {
        return citizen.Profession != null;
    }

    public override void BuildTasks(Citizen citizen)
    {
        if (citizen.WorkPlace == null)
        {
            // Сначала задача найти работу
            citizen.Tasks.Enqueue(new FindJobTask(_registry));
        }

        // Если работа найдена — перемещение и выполнение работы
        if (citizen.WorkPlace != null)
        {
            base.BuildTasks(citizen); // Создаёт PerformWorkTask через MoveAndPerformTaskScenario
        }
    }

    protected override ICitizenTask CreateMainTask(Citizen citizen)
    {
        return new PerformWorkTask(citizen);
    }
}