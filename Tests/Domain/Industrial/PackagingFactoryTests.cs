/*using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests.Domain.Industrial
{
    [TestClass]
    public class PackagingFactoryTests
    {
        [TestMethod]
        public void PackagingFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange
            var factory = new PackagingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(2, building.Floors);
            Assert.AreEqual(15, building.MaxOccupancy);
            Assert.AreEqual(new Area(6, 6), building.Area);
        }

        [TestMethod]
        public void PackagingFactory_CreatesBuilding_WithWorkshops()
        {
            // Arrange
            var factory = new PackagingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Workshops.Count > 0, "Завод должен иметь цеха");
            Assert.IsTrue(building.Workshops.Count >= 10, "Завод упаковки должен иметь минимум 10 цехов");
        }

        [TestMethod]
        public void PackagingFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new PackagingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(ProductType.CardboardSheets) || 
                         building.MaterialsBank.ContainsKey(NaturalResourceType.Glass) ||
                         building.MaterialsBank.ContainsKey(ProductType.Plastic),
                         "Должны быть начальные материалы для производства упаковки");
        }

        [TestMethod]
        public void PackagingFactory_Production_ProducesPackagingProducts()
        {
            // Arrange
            var factory = new PackagingFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();
            building.RunOnce();

            // Assert
            var hasPackagingProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.CardboardBox ||
                         p == ProductType.PlasticBottle ||
                         p == ProductType.GlassJar ||
                         p == ProductType.AluminiumCan ||
                         p == ProductType.WoodenCrate ||
                         p == ProductType.FoodContainer ||
                         p == ProductType.ShippingBox ||
                         p == ProductType.CosmeticBottle ||
                         p == ProductType.PharmaceuticalPack ||
                         p == ProductType.GiftBox);

            Assert.IsTrue(hasPackagingProducts || building.ProductsBank.Count > 0, 
                "После производства должны появиться продукты упаковки");
        }
    }
}
*/

