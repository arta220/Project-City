using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Services.BuildingRegistry;
using Services.Citizens.Scenarios.Base;
using Services.Citizens.Tasks.Job;
using Services.EntityMovement.Service;
using Services.Time;

namespace Services.Citizens.Scenarios.Work
{
    public class WorkScenario : MoveAndPerformTaskScenario
    {
        public WorkScenario(IEntityMovementService movement, IBuildingRegistry registry)
            : base(movement, registry)
        {
        }

        public override bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Работает только если гражданин трудоспособен,
            // но WorkPlace может быть пока не назначен
            return citizen.Profession != null;
        }

        public override void BuildTasks(Citizen citizen)
        {
            // 1. Если нет рабочего места — ищем работу
            if (citizen.WorkPlace == null)
            {
                citizen.Tasks.Enqueue(new FindJobTask(_registry));
                return;
            }

            // 2. Если работа есть — выполняем обычный MovePerformWork
            base.BuildTasks(citizen);
        }

        protected override Building GetTargetBuilding(Citizen citizen) => citizen.WorkPlace!;
        protected override ICitizenTask CreateMainTask(Citizen citizen, Building target) => new PerformWorkTask(citizen);
        protected override Building? GetReturnTarget(Citizen citizen) => null; // Можно куда-то вернуться, но удобнее будет в другом сценарии
    }
}
