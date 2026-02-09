using Domain.Buildings;
using Domain.Factories;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CommercialBuildingFactoryTests
    {
        [TestMethod]
        public void PharmacyFactory_ShouldCreatePharmacy()
        {
            // Arrange
            var factory = new PharmacyFactory();
            var area = new Area(1, 1);

            // Act
            var pharmacy = factory.Create();

            // Assert
            Assert.IsInstanceOfType(pharmacy, typeof(Pharmacy));
            Assert.AreEqual(1, pharmacy.Area.Width);
            Assert.AreEqual(1, pharmacy.Area.Height);
        }

        [TestMethod]
        public void AllCommercialFactories_ShouldCreateCorrectTypes()
        {
            // Arrange
            var factories = new object[]
            {
                new PharmacyFactory(),
                new ShopFactory(),
                new SupermarketFactory(),
                new CafeFactory(),
                new RestaurantFactory(),
                new GasStationFactory()
            };

            var area = new Area(2, 2);

            foreach (var factoryObj in factories)
            {
                if (factoryObj is IMapObjectFactory factory)
                {
                    // Act
                    var building = factory.Create();

                    // Assert
                    Assert.IsNotNull(building);
                    Assert.IsInstanceOfType(building, typeof(CommercialBuilding));
                    Assert.IsInstanceOfType(building, typeof(IServiceBuilding));
                }
            }
        }
    }
}