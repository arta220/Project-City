using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Citizens;
using Domain.Buildings.Residential;
using Domain.Map;
using Services.EntityMovement.Service;
using System.Linq;
using static MovementServiceTests;
using Tests.Mocks;

[TestClass]
public class CitizenHomeScenarioTests
{
    [TestMethod]
    public void CitizenMovesHomeSuccessfully()
    {
        var map = new FakeMap();
        var movement = new EntityMovementService(new FakePathFinder(), new FakeNavigationProfile());
        var home = new ResidentialBuilding(1, 5, new Area(2, 2));
        var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
        {
            NavigationProfile = new FakeNavigationProfile(),
            Home = home,
            Position = new Position(0, 0),
            TargetPosition = new Position(0, 0)
        };
        var homeEntrance = new Position(5, 5);

        movement.SetTarget(citizen, homeEntrance);
        while (citizen.Position != homeEntrance)
            movement.PlayMovement(citizen, new Domain.Common.Time.SimulationTime(1));

        Assert.AreEqual(homeEntrance, citizen.Position);
    }
}
