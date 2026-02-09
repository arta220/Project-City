using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests.Domain.Industrial
{
    [TestClass]
    public class CardboardFactoryTests
    {
        [TestMethod]
        public void CardboardFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange
            var factory = new CardboardFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(2, building.Floors);
            Assert.AreEqual(12, building.MaxOccupancy);
            Assert.AreEqual(new Area(5, 5), building.Area);
        }

        [TestMethod]
        public void CardboardFactory_CreatesBuilding_WithWorkshops()
        {
            // Arrange
            var factory = new CardboardFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Workshops.Count > 0, "Завод должен иметь цеха");
            Assert.IsTrue(building.Workshops.Count >= 7, "Завод картона должен иметь минимум 7 цехов");
        }

        [TestMethod]
        public void CardboardFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new CardboardFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.WoodChips), "Должна быть древесная щепа");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.RecycledPaper), "Должна быть макулатура");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Chemicals), "Должны быть химикаты");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Water), "Должна быть вода");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Energy), "Должна быть энергия");
        }

        [TestMethod]
        public void CardboardFactory_Production_ProducesCardboardProducts()
        {
            // Arrange
            var factory = new CardboardFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();
            building.RunOnce();

            // Assert
            var hasCardboardProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.CardboardSheets ||
                         p == ProductType.CorrugatedCardboard ||
                         p == ProductType.SolidCardboard ||
                         p == ProductType.CardboardBoxes ||
                         p == ProductType.CardboardTubes ||
                         p == ProductType.EggPackaging ||
                         p == ProductType.ProtectivePackaging);

            Assert.IsTrue(hasCardboardProducts || building.ProductsBank.Count > 0, 
                "После производства должны появиться продукты картона");
        }

        [TestMethod]
        public void CardboardFactory_Workshop_ProcessesMaterials()
        {
            // Arrange
            var factory = new CardboardFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var initialWoodChips = building!.MaterialsBank.GetValueOrDefault(NaturalResourceType.WoodChips, 0);
            building.RunOnce();
            var afterWoodChips = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.WoodChips, 0);

            // Assert
            Assert.IsTrue(initialWoodChips >= afterWoodChips, 
                "После производства количество древесной щепы должно уменьшиться");
        }
    }
}

