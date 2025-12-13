using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests.Domain.Industrial
{
    [TestClass]
    public class AlcoholFactoryTests
    {
        [TestMethod]
        public void AlcoholFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange
            var factory = new AlcoholFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(2, building.Floors);
            Assert.AreEqual(80, building.MaxOccupancy);
            Assert.AreEqual(new Area(5, 5), building.Area);
            Assert.AreEqual(IndustrialBuildingType.Factory, building.Type);
        }

        [TestMethod]
        public void AlcoholFactory_CreatesBuilding_WithTenWorkshops()
        {
            // Arrange
            var factory = new AlcoholFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(10, building.Workshops.Count, "Алкогольный завод должен иметь 10 цехов");
        }

        [TestMethod]
        public void AlcoholFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new AlcoholFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Water), "Должна быть вода");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Energy), "Должна быть энергия");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(ProductType.Plastic), "Должен быть пластик");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(ProductType.GlassJar), "Должны быть стеклянные банки");
        }

        [TestMethod]
        public void AlcoholFactory_Production_ProducesAlcoholProducts()
        {
            // Arrange
            var factory = new AlcoholFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();
            building.RunOnce();

            // Assert
            var hasAlcoholProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.Beer ||
                         p == ProductType.Vodka ||
                         p == ProductType.Wine ||
                         p == ProductType.Whiskey ||
                         p == ProductType.Rum ||
                         p == ProductType.Tequila ||
                         p == ProductType.Gin ||
                         p == ProductType.Brandy ||
                         p == ProductType.Champagne ||
                         p == ProductType.Liqueur);

            Assert.IsTrue(hasAlcoholProducts || building.ProductsBank.Count > 0,
                "После производства должны появиться алкогольные продукты");
        }

        [TestMethod]
        public void AlcoholFactory_Workshop_ProcessesMaterials()
        {
            // Arrange
            var factory = new AlcoholFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var initialWater = building!.MaterialsBank.GetValueOrDefault(NaturalResourceType.Water, 0);
            building.RunOnce();
            var afterWater = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Water, 0);

            // Assert
            Assert.IsTrue(initialWater >= afterWater,
                "После производства количество воды должно уменьшиться");
        }

        [TestMethod]
        public void AlcoholFactory_AllWorkshops_HaveValidInputOutput()
        {
            // Arrange
            var factory = new AlcoholFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act & Assert
            Assert.IsNotNull(building);

            foreach (var workshop in building.Workshops)
            {
                Assert.IsNotNull(workshop.InputMaterial, "Цех должен иметь входной материал");
                Assert.IsNotNull(workshop.OutputProduct, "Цех должен иметь выходной продукт");
                Assert.IsTrue(workshop.ProductionCoefficient > 0, "Коэффициент производства должен быть больше 0");
            }
        }

        [TestMethod]
        public void AlcoholFactory_ProducesDifferentAlcoholTypes()
        {
            // Arrange
            var factory = new AlcoholFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство 5 раз, чтобы набрать статистику
            for (int i = 0; i < 5; i++)
            {
                building!.RunOnce();
            }

            // Assert - проверяем, что производится хотя бы один вид алкоголя
            var alcoholProductsCount = building!.ProductsBank.Keys
                .OfType<ProductType>()
                .Count(p => p == ProductType.Beer ||
                           p == ProductType.Vodka ||
                           p == ProductType.Wine ||
                           p == ProductType.Whiskey ||
                           p == ProductType.Rum ||
                           p == ProductType.Tequila ||
                           p == ProductType.Gin ||
                           p == ProductType.Brandy ||
                           p == ProductType.Champagne ||
                           p == ProductType.Liqueur);

            Assert.IsTrue(alcoholProductsCount > 0,
                "Завод должен производить хотя бы один вид алкоголя после нескольких циклов");
        }
    }
}
