using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Citizens.Movement;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.CitizenSimulation.Tasks;
using Services.Time;
using Services.Utilities;

namespace Services.Citizens.Scenarios
{
    public class UtilityWorkerScenario : ICitizenScenario
    {
        private readonly IBuildingRegistry _registry;
        private readonly IUtilityService _utilityService;
        private readonly ICitizenMovementService _movement;

        public UtilityWorkerScenario(
            IBuildingRegistry registry,
            IUtilityService utilityService,
            ICitizenMovementService movement)
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
            var officeEntrance = GetEntrance(citizen.WorkPlace);
            citizen.Tasks.Enqueue(new MoveToPositionTask(officeEntrance, _movement));

            // 2. Находим здание с поломанной коммуналкой
            var buildingToRepair = FindBuildingToRepair();
            if (buildingToRepair != null)
            {
                var repairPosition = GetRepairPosition(buildingToRepair);
                // 3. Таска идти к месту ремонта
                citizen.Tasks.Enqueue(new MoveToPositionTask(repairPosition, _movement));
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

        private Position GetEntrance(MapObject building)
        {
            var (placement, ok) = _registry.TryGetPlacement(building);
            return ok ? placement.Value.Entrance : new Position(0, 0);
        }

        private Position GetRepairPosition(MapObject building)
        {
            var (placement, ok) = _registry.TryGetPlacement(building);
            if (!ok) return new Position(0, 0);

            var entrance = placement.Value.Entrance;
            return new Position(entrance.X + 1, entrance.Y);
        }
    }
}