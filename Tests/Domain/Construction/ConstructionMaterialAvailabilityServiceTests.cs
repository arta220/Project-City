using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Construction;
using Domain.Buildings.Construction;
using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Map;
using Tests.Mocks;
using System.Collections.Generic;

namespace Tests.Domain.Construction
{
    [TestClass]
    public class ConstructionMaterialAvailabilityServiceTests
    {
        private ConstructionMaterialAvailabilityService _service = null!;
        private FakeBuildingRegistry _buildingRegistry = null!;
        private ConstructionProject _project = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _buildingRegistry = new FakeBuildingRegistry();

            // Создаем проект с требованиями
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Cement, 100 },
                { ConstructionMaterialType.Bricks, 200 },
                { ConstructionMaterialType.Sand, 50 }
            };
            _project = new ConstructionProject(
                new Domain.Factories.SmallHouseFactory(),
                requiredMaterials
            );

            _service = new ConstructionMaterialAvailabilityService(_buildingRegistry);
        }

        [TestMethod]
        public void CheckMaterialAvailability_ReturnsAllMaterialsAvailable_WhenAllMaterialsPresent()
        {
            // Arrange
            // Создаем заводы с необходимыми материалами
            var cementFactory = CreateCementFactoryWithStock();
            var brickFactory = CreateBrickFactoryWithStock();
            var sandFactory = CreateSandFactoryWithStock();

            _buildingRegistry.Add(cementFactory);
            _buildingRegistry.Add(brickFactory);
            _buildingRegistry.Add(sandFactory);

            // Act
            var info = _service.CheckMaterialAvailability(_project);

            // Assert
            Assert.IsTrue(info.AllMaterialsAvailable);
            Assert.AreEqual(0, info.UnavailableMaterials.Count);
        }

        [TestMethod]
        public void CheckMaterialAvailability_ReturnsUnavailableMaterials_WhenMaterialsMissing()
        {
            // Arrange
            // Создаем только цементный завод
            var cementFactory = CreateCementFactoryWithStock();
            _buildingRegistry.Add(cementFactory);

            // Act
            var info = _service.CheckMaterialAvailability(_project);

            // Assert
            Assert.IsFalse(info.AllMaterialsAvailable);
            Assert.AreEqual(2, info.UnavailableMaterials.Count); // Bricks и Sand недоступны
            Assert.IsTrue(info.UnavailableMaterials.ContainsKey(ConstructionMaterialType.Bricks));
            Assert.IsTrue(info.UnavailableMaterials.ContainsKey(ConstructionMaterialType.Sand));
            Assert.IsFalse(info.UnavailableMaterials.ContainsKey(ConstructionMaterialType.Cement));
        }

        [TestMethod]
        public void CheckMaterialAvailability_IncludesFactoriesThatProduceMissingMaterials()
        {
            // Arrange
            var cementFactory = CreateCementFactoryWithStock();
            _buildingRegistry.Add(cementFactory);

            // Act
            var info = _service.CheckMaterialAvailability(_project);

            // Assert
            Assert.IsTrue(info.UnavailableMaterials.ContainsKey(ConstructionMaterialType.Bricks));
            var brickFactories = info.UnavailableMaterials[ConstructionMaterialType.Bricks];
            Assert.IsTrue(brickFactories.Contains("Кирпичный завод"));

            Assert.IsTrue(info.UnavailableMaterials.ContainsKey(ConstructionMaterialType.Sand));
            var sandFactories = info.UnavailableMaterials[ConstructionMaterialType.Sand];
            Assert.IsTrue(sandFactories.Contains("Завод")); // Для Steel возвращается "Завод"
        }

        [TestMethod]
        public void GetFactoriesProducingMaterial_ReturnsCorrectFactories_ForCement()
        {
            // Act
            var factories = _service.GetFactoriesProducingMaterial(ConstructionMaterialType.Cement);

            // Assert
            Assert.AreEqual(1, factories.Count);
            Assert.AreEqual("Цементный завод", factories[0]);
        }

        [TestMethod]
        public void GetFactoriesProducingMaterial_ReturnsCorrectFactories_ForBricks()
        {
            // Act
            var factories = _service.GetFactoriesProducingMaterial(ConstructionMaterialType.Bricks);

            // Assert
            Assert.AreEqual(1, factories.Count);
            Assert.AreEqual("Кирпичный завод", factories[0]);
        }

        [TestMethod]
        public void GetFactoriesProducingMaterial_ReturnsCorrectFactories_ForSteel()
        {
            // Act
            var factories = _service.GetFactoriesProducingMaterial(ProductType.Steel);

            // Assert
            Assert.AreEqual(1, factories.Count);
            Assert.AreEqual("Завод", factories[0]);
        }

        [TestMethod]
        public void GetFactoriesProducingMaterial_ReturnsCorrectFactories_ForWood()
        {
            // Act
            var factories = _service.GetFactoriesProducingMaterial(NaturalResourceType.Wood);

            // Assert
            Assert.AreEqual(1, factories.Count);
            Assert.AreEqual("Склад", factories[0]);
        }

        [TestMethod]
        public void GetFactoriesProducingMaterial_ReturnsCorrectFactories_ForGlass()
        {
            // Act
            var factories = _service.GetFactoriesProducingMaterial(NaturalResourceType.Glass);

            // Assert
            Assert.AreEqual(1, factories.Count);
            Assert.AreEqual("Завод упаковки", factories[0]);
        }

        [TestMethod]
        public void CheckMaterialAvailability_ReturnsEmptyInfo_WhenProjectIsNull()
        {
            // Act
            var info = _service.CheckMaterialAvailability(null!);

            // Assert
            Assert.IsTrue(info.AllMaterialsAvailable);
            Assert.AreEqual(0, info.UnavailableMaterials.Count);
        }

        [TestMethod]
        public void CheckMaterialAvailability_ReturnsAllAvailable_WhenNoMaterialsRequired()
        {
            // Arrange
            var emptyProject = new ConstructionProject(
                new Domain.Factories.SmallHouseFactory(),
                new Dictionary<Enum, int>()
            );

            // Act
            var info = _service.CheckMaterialAvailability(emptyProject);

            // Assert
            Assert.IsTrue(info.AllMaterialsAvailable);
            Assert.AreEqual(0, info.UnavailableMaterials.Count);
        }

        [TestMethod]
        public void CheckMaterialAvailability_FindsMaterialsInExistingFactories()
        {
            // Arrange
            // Создаем завод с цементом в ProductsBank
            var cementFactory = new IndustrialBuilding(2, 40, new Area(5, 5), IndustrialBuildingType.Factory);
            cementFactory.ProductsBank[ConstructionMaterialType.Cement] = 150;
            _buildingRegistry.Add(cementFactory);

            var projectWithCement = new ConstructionProject(
                new Domain.Factories.SmallHouseFactory(),
                new Dictionary<Enum, int> { { ConstructionMaterialType.Cement, 100 } }
            );

            // Act
            var info = _service.CheckMaterialAvailability(projectWithCement);

            // Assert
            Assert.IsTrue(info.AllMaterialsAvailable);
            Assert.AreEqual(0, info.UnavailableMaterials.Count);
        }

        [TestMethod]
        public void CheckMaterialAvailability_FindsMaterialsInMaterialsBank()
        {
            // Arrange
            // Создаем завод с кирпичами в MaterialsBank
            var brickFactory = new IndustrialBuilding(1, 30, new Area(4, 4), IndustrialBuildingType.Factory);
            brickFactory.MaterialsBank[ConstructionMaterialType.Bricks] = 250;
            _buildingRegistry.Add(brickFactory);

            var projectWithBricks = new ConstructionProject(
                new Domain.Factories.SmallHouseFactory(),
                new Dictionary<Enum, int> { { ConstructionMaterialType.Bricks, 100 } }
            );

            // Act
            var info = _service.CheckMaterialAvailability(projectWithBricks);

            // Assert
            Assert.IsTrue(info.AllMaterialsAvailable);
            Assert.AreEqual(0, info.UnavailableMaterials.Count);
        }

        // Вспомогательные методы для создания тестовых заводов
        private IndustrialBuilding CreateCementFactoryWithStock()
        {
            var factory = new IndustrialBuilding(2, 40, new Area(5, 5), IndustrialBuildingType.Factory);
            factory.AddWorkshop(NaturalResourceType.Limestone, ConstructionMaterialType.Cement, 5);
            factory.MaterialsBank[NaturalResourceType.Limestone] = 100;
            factory.RunOnce(); // Производим цемент
            return factory;
        }

        private IndustrialBuilding CreateBrickFactoryWithStock()
        {
            var factory = new IndustrialBuilding(1, 30, new Area(4, 4), IndustrialBuildingType.Factory);
            factory.AddWorkshop(NaturalResourceType.Clay, ConstructionMaterialType.Bricks, 8);
            factory.MaterialsBank[NaturalResourceType.Clay] = 200;
            factory.RunOnce(); // Производим кирпичи
            return factory;
        }

        private IndustrialBuilding CreateSandFactoryWithStock()
        {
            var factory = new IndustrialBuilding(1, 25, new Area(4, 5), IndustrialBuildingType.Factory);
            factory.MaterialsBank[NaturalResourceType.Sand] = 100;
            return factory;
        }
    }
}
