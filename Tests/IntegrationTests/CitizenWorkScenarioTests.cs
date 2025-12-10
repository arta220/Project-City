using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Map;
using Services.EntityMovement.Service;
using Tests.Mocks;

[TestClass]
public class CitizenWorkScenarioTests
{
    [TestMethod]
    public void CitizenMovesToWorkSuccessfully()
    {
        var movement = new EntityMovementService(new FakePathFinder(), new FakeNavigationProfile());
        var workplace = new ResidentialBuilding(1, 5, new Area(2, 2));
        var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
        {
            NavigationProfile = new FakeNavigationProfile(),
            WorkPlace = workplace,
            Position = new Position(0, 0),
            TargetPosition = new Position(0, 0)
        };
        var workEntrance = new Position(10, 10);

        movement.SetTarget(citizen, workEntrance);
        while (citizen.Position != workEntrance)
            movement.PlayMovement(citizen, new Domain.Common.Time.SimulationTime(1));

        Assert.AreEqual(workEntrance, citizen.Position);
    }
}
