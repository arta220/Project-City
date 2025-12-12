/*using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Citizens;
using Domain.Buildings.Residential;
using Domain.Map;

[TestClass]
public class CitizenTests
{
    [TestMethod]
    public void TestCitizenCreation()
    {
        var area = new Area(1, 1);
        var citizen = new MovingEntity(area, speed: 1.0f);
        var home = new ResidentialBuilding(1, 5, new Area(2, 2));
        citizen.Home = home;

        Assert.AreEqual(home, citizen.Home);
        Assert.AreEqual(1.0f, citizen.Speed);
    }
}
*/
