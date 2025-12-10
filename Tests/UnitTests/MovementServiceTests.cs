using Domain.Citizens;
using Domain.Map;
using Services.EntityMovement.Service;

[TestClass]
public class MovementServiceTests
{

    [TestMethod]
    public void TestMovementToTarget()
    {
        var navigation = new FakeNavigationProfile();
        var movement = new EntityMovementService(new FakePathFinder(), navigation);
        var citizen = new Citizen(new Area(1, 1), 1.0f)
        {
            NavigationProfile = navigation,
            Position = new Position(0, 0),
            TargetPosition = new Position(0, 0)
        };
        var target = new Position(5, 5);

        movement.SetTarget(citizen, target);

        while (citizen.CurrentPath.Count > 0)
        {
            movement.PlayMovement(citizen, new Domain.Common.Time.SimulationTime(1));
        }

        Assert.AreEqual(target.X, citizen.Position.X);
        Assert.AreEqual(target.Y, citizen.Position.Y);
    }

}
