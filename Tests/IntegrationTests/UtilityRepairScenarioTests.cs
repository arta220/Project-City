using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Citizens.Job.Movement;
using Services.Utilities;
using Tests.Mocks;

[TestClass]
public class UtilityRepairScenarioTests
{
    [TestMethod]
    public void CitizenRepairsBrokenUtility()
    {
        var movement = new MovementService(new FakePathFinder());
        var utilityService = new FakeUtilityService();
        var residential = new ResidentialBuilding(1, 5, new Area(2, 2));
        utilityService.BreakUtilityForTesting(residential, UtilityType.Electricity, currentTick: 1);

        var citizen = new MovingEntity(new Area(1, 1), speed: 1.0f)
        {
            Position = new Position(0, 0)
        };
        var repairPosition = new Position(5, 5);

        movement.SetTarget(citizen, repairPosition);
        while (citizen.Position != repairPosition)
            movement.PlayMovement(citizen, new Domain.Common.Time.SimulationTime(1));

        utilityService.FixUtility(residential, UtilityType.Electricity);

        Assert.IsEmpty(utilityService.GetBrokenUtilities(residential));
        Assert.AreEqual(repairPosition, citizen.Position);
    }
}

public class FakeUtilityService : IUtilityService
{
    private readonly Dictionary<ResidentialBuilding, HashSet<UtilityType>> _broken = new();
    public void BreakUtilityForTesting(ResidentialBuilding building, UtilityType utilityType, int currentTick)
    {
        if (!_broken.ContainsKey(building)) _broken[building] = new HashSet<UtilityType>();
        _broken[building].Add(utilityType);
    }

    public void FixUtility(ResidentialBuilding building, UtilityType utilityType)
    {
        if (_broken.ContainsKey(building))
            _broken[building].Remove(utilityType);
    }

    public void FixAllUtilities(ResidentialBuilding building) => _broken[building]?.Clear();

    public Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building) =>
        _broken.ContainsKey(building) ? _broken[building].ToDictionary(u => u, u => 1) : new Dictionary<UtilityType, int>();

    public UtilityStatistics GetStatistics() => new UtilityStatistics();
    public void Update() { }

    public void Update(SimulationTime time)
    {
        throw new NotImplementedException();
    }
}
