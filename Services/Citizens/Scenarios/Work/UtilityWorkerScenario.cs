using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.EntityMovement.Service;
using Services.Time;
using Services.Utilities;

public class UtilityWorkerScenario : ICitizenScenario
{
    private readonly IEntityMovementService _movement;
    private readonly IBuildingRegistry _registry;
    private readonly IUtilityService _utilityService;

    public UtilityWorkerScenario(IEntityMovementService movement, IBuildingRegistry registry, IUtilityService utilityService)
    {
        _movement = movement;
        _registry = registry;
        _utilityService = utilityService;
    }

    public bool CanRun(Citizen citizen, ISimulationTimeService time) => citizen.Profession == CitizenProfession.UtilityWorker;

    public void BuildTasks(Citizen citizen)
    {
        // 1. Дойти до офиса
        citizen.Tasks.Enqueue(new MoveToBuildingTask(citizen.WorkPlace, _movement, _registry));

        // 2. Найти сломанные жилые здания
        var brokenHouses = _registry.GetBuildings<ResidentialBuilding>()
            .Where(b => b.Utilities.HasBrokenUtilities);

        foreach (var house in brokenHouses)
        {
            citizen.Tasks.Enqueue(new MoveToBuildingTask(house, _movement, _registry));
            citizen.Tasks.Enqueue(new RepairBuildingTask(house, _utilityService));
        }

        // 3. Вернуться в офис или в idle
        citizen.Tasks.Enqueue(new MoveToBuildingTask(citizen.WorkPlace, _movement, _registry));
    }
}
