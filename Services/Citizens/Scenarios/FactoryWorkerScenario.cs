using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.EntityMovement.Service;
using Services.Time;

namespace Services.Citizens.Scenarios
{
    public class FactoryWorkerScenario : ICitizenScenario
    {
        private readonly IBuildingRegistry _registry;
        private readonly IEntityMovementService _movement;

        public FactoryWorkerScenario(
            IBuildingRegistry registry,
            IEntityMovementService movement)
        {
            _registry = registry;
            _movement = movement;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Работник завода должен иметь место работы
            if (citizen.Profession != CitizenProfession.FactoryWorker || citizen.WorkPlace == null)
                return false;

            // Если рабочее время и рабочий не на работе - идем на работу
            if (time.IsWorkTime() && !IsAtWork(citizen))
                return true;

            // Если рабочий на работе и рабочее время - остаемся работать
            if (time.IsWorkTime() && IsAtWork(citizen) && citizen.State != CitizenState.Working)
            {
                citizen.State = CitizenState.Working;
                return false; // Не создаем новые задачи, просто меняем состояние
            }

            // Если не рабочее время и рабочий на работе - идем домой
            if (!time.IsWorkTime() && IsAtWork(citizen))
                return true; // Создадим задачу идти домой (если HomeScenario сработает позже)

            return false;
        }

        public void BuildTasks(Citizen citizen)
        {
            // Если рабочее время - идем на работу
            if (citizen.State != CitizenState.Working)
            {
                citizen.State = CitizenState.GoingWork;
                citizen.Tasks.Enqueue(new MoveToBuildingTask(citizen.WorkPlace, _movement, _registry));
            }
        }

        private bool IsAtWork(Citizen citizen)
        {
            var (placement, ok) = _registry.TryGetPlacement(citizen.WorkPlace);
            return ok && citizen.Position == placement.Value.Entrance;
        }
    }
}
