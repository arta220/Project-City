using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.EntityMovement.Service;
using Services.Time;
using Services.Utilities;

namespace Services.Citizens.Scenarios
{
    public class UtilityWorkerScenario : ICitizenScenario
    {
        private readonly IBuildingRegistry _registry;
        private readonly IUtilityService _utilityService;
        private readonly IEntityMovementService _movement;

        public UtilityWorkerScenario(
            IBuildingRegistry registry,
            IUtilityService utilityService,
            IEntityMovementService movement)
        {
            _registry = registry;
            _utilityService = utilityService;
            _movement = movement;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Работник должен быть на работе (офис)
            return citizen.Profession == CitizenProfession.UtilityWorker &&
                   citizen.WorkPlace != null &&
                   !IsAtWork(citizen);
        }

        public void BuildTasks(Citizen citizen)
        {
            citizen.State = CitizenState.GoingWork;

            // 1. Сначала идем в офис
            citizen.Tasks.Enqueue(new MoveToBuildingTask(citizen.WorkPlace, _movement, _registry));

            // 2. Находим здание с поломанной коммуналкой
            var buildingToRepair = FindBuildingToRepair();
            if (buildingToRepair != null)
            {

                // 3. Таска идти к месту ремонта
                citizen.Tasks.Enqueue(new MoveToBuildingTask(buildingToRepair, _movement, _registry));
                // 4. Таска ремонтировать
                citizen.Tasks.Enqueue(new RepairBuildingTask(buildingToRepair, _utilityService));
            }
        }

        private ResidentialBuilding? FindBuildingToRepair()
        {
            foreach (var building in _registry.GetBuildings<ResidentialBuilding>())
            {
                if (_utilityService.GetBrokenUtilities(building).Any())
                    return building;
            }
            return null;
        }


        private bool IsAtWork(Citizen citizen)
        {
            var (placement, ok) = _registry.TryGetPlacement(citizen.WorkPlace);
            return ok && citizen.Position == placement.Value.Entrance;
        }
    }
}
