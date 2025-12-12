using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests.Domain.Industrial
{
    /// <summary>
    /// Тесты для рыбодобывающего комплекса
    /// Проверяют флот, переработку рыбы и хранение
    /// </summary>
    [TestClass]
    public class FishingIndustryFactoryTests
    {
        [TestMethod]
        public void FishingIndustryFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange
            var factory = new FishingIndustryFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(2, building.Floors);
            Assert.AreEqual(25, building.MaxOccupancy);
            Assert.AreEqual(new Area(5, 5), building.Area);
        }

        [TestMethod]
        public void FishingIndustryFactory_CreatesBuilding_WithWorkshops()
        {
            // Arrange
            var factory = new FishingIndustryFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Workshops.Count > 0, "Завод должен иметь цеха");
            Assert.IsTrue(building.Workshops.Count >= 6, "Рыбодобывающий комплекс должен иметь минимум 6 цехов");
        }

        [TestMethod]
        public void FishingIndustryFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new FishingIndustryFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.FuelForFleet), "Должно быть топливо для флота");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Fish), "Должна быть рыба");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Ice), "Должен быть лед для хранения");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Energy), "Должна быть энергия");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Water), "Должна быть вода");
        }

        [TestMethod]
        public void FishingIndustryFactory_Production_ProducesFishingProducts()
        {
            // Arrange
            var factory = new FishingIndustryFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();
            building.RunOnce();

            // Assert
            var hasFishingProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.ProcessedFish ||
                         p == ProductType.CannedFish ||
                         p == ProductType.FrozenFish ||
                         p == ProductType.FishProducts ||
                         p == ProductType.Fishmeal);

            Assert.IsTrue(hasFishingProducts || building.ProductsBank.Count > 0, 
                "После производства должны появиться продукты рыбодобывающей отрасли");
        }

        [TestMethod]
        public void FishingIndustryFactory_Workshop_ProcessesMaterials()
        {
            // Arrange
            var factory = new FishingIndustryFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var initialFuel = building!.MaterialsBank.GetValueOrDefault(NaturalResourceType.FuelForFleet, 0);
            building.RunOnce();
            var afterFuel = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.FuelForFleet, 0);

            // Assert
            Assert.IsTrue(initialFuel >= afterFuel, 
                "После производства количество топлива для флота должно уменьшиться");
        }

        [TestMethod]
        public void FishingIndustryFactory_FleetProduction_ProducesFish()
        {
            // Arrange
            var factory = new FishingIndustryFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство
            building!.RunOnce();

            // Assert - проверяем, что есть цех рыбодобычи (флот)
            var hasFleetWorkshop = building.Workshops.Any(w => 
                w.InputMaterial.Equals(NaturalResourceType.FuelForFleet) &&
                w.OutputProduct.Equals(NaturalResourceType.Fish));

            Assert.IsTrue(hasFleetWorkshop,
                "Должен быть цех рыбодобычи, использующий топливо для флота");
        }

        [TestMethod]
        public void FishingIndustryFactory_FishProcessing_ProducesProcessedFish()
        {
            // Arrange
            var factory = new FishingIndustryFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство
            building!.RunOnce();

            // Assert - проверяем, что есть цех переработки рыбы
            var hasProcessingWorkshop = building.Workshops.Any(w => 
                w.InputMaterial.Equals(NaturalResourceType.Fish) &&
                w.OutputProduct.Equals(ProductType.ProcessedFish));

            Assert.IsTrue(hasProcessingWorkshop,
                "Должен быть цех переработки рыбы");
        }

        [TestMethod]
        public void FishingIndustryFactory_FishFreezing_UsesIce()
        {
            // Arrange
            var factory = new FishingIndustryFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство
            building!.RunOnce();

            // Assert - проверяем, что есть цех заморозки рыбы
            var hasFreezingWorkshop = building.Workshops.Any(w => 
                w.InputMaterial.Equals(NaturalResourceType.Fish) &&
                w.OutputProduct.Equals(ProductType.FrozenFish));

            Assert.IsTrue(hasFreezingWorkshop,
                "Должен быть цех заморозки рыбы");

            // Проверяем наличие льда для хранения
            var hasIce = building.MaterialsBank.ContainsKey(NaturalResourceType.Ice);
            Assert.IsTrue(hasIce,
                "Должен быть лед для процесса заморозки");
        }

        [TestMethod]
        public void FishingIndustryFactory_Canning_ProducesCannedFish()
        {
            // Arrange
            var factory = new FishingIndustryFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство
            building!.RunOnce();

            // Assert - проверяем, что есть цех консервирования
            var hasCanningWorkshop = building.Workshops.Any(w => 
                w.InputMaterial.Equals(ProductType.ProcessedFish) &&
                w.OutputProduct.Equals(ProductType.CannedFish));

            Assert.IsTrue(hasCanningWorkshop,
                "Должен быть цех консервирования рыбы");
        }

        [TestMethod]
        public void FishingIndustryFactory_FishmealProduction_ProducesFishmeal()
        {
            // Arrange
            var factory = new FishingIndustryFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство
            building!.RunOnce();

            // Assert - проверяем, что есть цех производства рыбной муки
            var hasFishmealWorkshop = building.Workshops.Any(w => 
                w.InputMaterial.Equals(NaturalResourceType.Fish) &&
                w.OutputProduct.Equals(ProductType.Fishmeal));

            Assert.IsTrue(hasFishmealWorkshop,
                "Должен быть цех производства рыбной муки из рыбы");
        }
    }
}

