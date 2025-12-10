using Domain.Citizens;
using Domain.Map;
using Services.Citizens.Movement;

[TestClass]
public class MovementServiceTests
{

    [TestMethod]
    public void TestMovementToTarget()
    {
        var movement = new MovementService(new FakePathFinder());
        var citizen = new MovingEntity(new Area(1, 1), 1.0f);
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
