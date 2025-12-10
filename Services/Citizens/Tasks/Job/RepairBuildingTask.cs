using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Services.Utilities;

namespace Services.Citizens.Tasks.Job
{
    public class RepairBuildingTask : ICitizenTask
    {
        private readonly ResidentialBuilding _building;
        private readonly IUtilityService _utilityService;

        public bool IsCompleted { get; private set; }

        public RepairBuildingTask(ResidentialBuilding building, IUtilityService utilityService)
        {
            _building = building;
            _utilityService = utilityService;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted) return;

            var broken = _utilityService.GetBrokenUtilities(_building);
            foreach (var type in broken.Keys)
                _utilityService.FixUtility(_building, type);

            IsCompleted = true;
        }
    }
}
