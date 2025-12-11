using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Map;
using System.Collections.Generic;

namespace Tests.Domain.Construction
{
    [TestClass]
    public class IndustrialBuildingTests
    {
        [TestMethod]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            var area = new Area(5, 5);

            // Act
            var building = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 40,
                area: area,
                type: IndustrialBuildingType.Factory
            );

            // Assert
            Assert.AreEqual(2, building.Floors);
            Assert.AreEqual(40, building.MaxOccupancy);
            Assert.AreEqual(area, building.Area);
            Assert.AreEqual(IndustrialBuildingType.Factory, building.Type);
            Assert.IsNotNull(building.MaterialsBank);
            Assert.IsNotNull(building.ProductsBank);
            Assert.IsNotNull(building.Workshops);
            Assert.AreEqual(0, building.Workshops.Count);
        }

        [TestMethod]
        public void AddWorkshop_AddsWorkshopToCollection()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);

            // Act
            building.AddWorkshop(
                NaturalResourceType.Clay,
                ConstructionMaterialType.Bricks,
                coeff: 8
            );

            // Assert
            Assert.AreEqual(1, building.Workshops.Count);
            var workshop = building.Workshops[0];
            Assert.AreEqual(NaturalResourceType.Clay, workshop.InputMaterial);
            Assert.AreEqual(ConstructionMaterialType.Bricks, workshop.OutputProduct);
            Assert.AreEqual(8, workshop.ProductionCoefficient);
        }

        [TestMethod]
        public void AddWorkshop_AddsMultipleWorkshops()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);

            // Act
            building.AddWorkshop(NaturalResourceType.Limestone, ConstructionMaterialType.Cement, 5);
            building.AddWorkshop(NaturalResourceType.Clay, ConstructionMaterialType.Cement, 3);
            building.AddWorkshop(NaturalResourceType.Sand, ConstructionMaterialType.Concrete, 6);

            // Assert
            Assert.AreEqual(3, building.Workshops.Count);
        }

        [TestMethod]
        public void RunOnce_ProcessesAllWorkshops()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);

            // Добавим материалы для производства
            building.MaterialsBank[NaturalResourceType.Clay] = 16; // Достаточно для 2 кирпичей

            // Добавим цех
            building.AddWorkshop(NaturalResourceType.Clay, ConstructionMaterialType.Bricks, 8);

            // Act
            building.RunOnce();

            // Assert
            Assert.AreEqual(8, building.MaterialsBank[NaturalResourceType.Clay]); // Потратили 8
            Assert.AreEqual(8, building.ProductsBank[ConstructionMaterialType.Bricks]); // Произвели 8
        }

        [TestMethod]
        public void RunOnce_ProcessesMultipleWorkshops()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);

            // Добавим материалы
            building.MaterialsBank[NaturalResourceType.Limestone] = 10;
            building.MaterialsBank[NaturalResourceType.Clay] = 6;

            // Добавим цехи
            building.AddWorkshop(NaturalResourceType.Limestone, ConstructionMaterialType.Cement, 5);
            building.AddWorkshop(NaturalResourceType.Clay, ConstructionMaterialType.Bricks, 3);

            // Act
            building.RunOnce();

            // Assert
            Assert.AreEqual(5, building.MaterialsBank[NaturalResourceType.Limestone]); // Потратили 5
            Assert.AreEqual(3, building.MaterialsBank[NaturalResourceType.Clay]); // Потратили 3
            Assert.AreEqual(5, building.ProductsBank[ConstructionMaterialType.Cement]); // Произвели 5 цемента
            Assert.AreEqual(3, building.ProductsBank[ConstructionMaterialType.Bricks]); // Произвели 3 кирпича
        }

        [TestMethod]
        public void RunOnce_DoesNothing_WhenNoMaterials()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);

            // Добавим цех, но не добавим материалы
            building.AddWorkshop(NaturalResourceType.Clay, ConstructionMaterialType.Bricks, 8);

            // Act
            building.RunOnce();

            // Assert
            Assert.AreEqual(0, building.MaterialsBank.Count);
            Assert.AreEqual(0, building.ProductsBank.Count);
        }

        [TestMethod]
        public void RunOnce_ProcessesWorkshop_WhenMaterialsInProductsBank()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);

            // Добавим материал в ProductsBank (мог попасть туда от предыдущего производства)
            building.ProductsBank[NaturalResourceType.Clay] = 16;

            // Добавим цех, который использует материал из ProductsBank
            building.AddWorkshop(NaturalResourceType.Clay, ConstructionMaterialType.Bricks, 8);

            // Act
            building.RunOnce();

            // Assert
            Assert.AreEqual(8, building.ProductsBank[NaturalResourceType.Clay]); // Потратили 8 из ProductsBank
            Assert.AreEqual(8, building.ProductsBank[ConstructionMaterialType.Bricks]); // Произвели 8 кирпичей
        }
    }

    [TestClass]
    public class WorkshopTests
    {
        [TestMethod]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);

            // Act
            var workshop = new IndustrialBuilding.Workshop(
                building,
                NaturalResourceType.Clay,
                ConstructionMaterialType.Bricks,
                8
            );

            // Assert
            Assert.AreEqual(NaturalResourceType.Clay, workshop.InputMaterial);
            Assert.AreEqual(ConstructionMaterialType.Bricks, workshop.OutputProduct);
            Assert.AreEqual(8, workshop.ProductionCoefficient);
        }

        [TestMethod]
        public void Process_ReturnsTrue_AndProducesOutput_WhenInputAvailableInMaterialsBank()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);
            building.MaterialsBank[NaturalResourceType.Clay] = 10;

            var workshop = new IndustrialBuilding.Workshop(
                building,
                NaturalResourceType.Clay,
                ConstructionMaterialType.Bricks,
                8
            );

            // Act
            var result = workshop.Process();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, building.MaterialsBank[NaturalResourceType.Clay]); // 10 - 8 = 2
            Assert.AreEqual(8, building.ProductsBank[ConstructionMaterialType.Bricks]); // Произвели 8
        }

        [TestMethod]
        public void Process_ReturnsTrue_AndProducesOutput_WhenInputAvailableInProductsBank()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);
            building.ProductsBank[NaturalResourceType.Clay] = 10; // Материал в ProductsBank

            var workshop = new IndustrialBuilding.Workshop(
                building,
                NaturalResourceType.Clay,
                ConstructionMaterialType.Bricks,
                8
            );

            // Act
            var result = workshop.Process();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, building.ProductsBank[NaturalResourceType.Clay]); // 10 - 8 = 2
            Assert.AreEqual(8, building.ProductsBank[ConstructionMaterialType.Bricks]); // Произвели 8
        }

        [TestMethod]
        public void Process_ReturnsFalse_WhenNoInputMaterial()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);

            var workshop = new IndustrialBuilding.Workshop(
                building,
                NaturalResourceType.Clay,
                ConstructionMaterialType.Bricks,
                8
            );

            // Act
            var result = workshop.Process();

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, building.MaterialsBank.Count);
            Assert.AreEqual(0, building.ProductsBank.Count);
        }

        [TestMethod]
        public void Process_ReturnsFalse_WhenInsufficientInputMaterial()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);
            building.MaterialsBank[NaturalResourceType.Clay] = 5; // Недостаточно (нужно 8)

            var workshop = new IndustrialBuilding.Workshop(
                building,
                NaturalResourceType.Clay,
                ConstructionMaterialType.Bricks,
                8
            );

            // Act
            var result = workshop.Process();

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(5, building.MaterialsBank[NaturalResourceType.Clay]); // Материал не потрачен
            Assert.AreEqual(0, building.ProductsBank.Count);
        }

        [TestMethod]
        public void Process_IncreasesExistingProduct_WhenProductAlreadyExists()
        {
            // Arrange
            var building = new IndustrialBuilding(1, 10, new Area(3, 3), IndustrialBuildingType.Factory);
            building.MaterialsBank[NaturalResourceType.Clay] = 16; // Достаточно для 2 производств
            building.ProductsBank[ConstructionMaterialType.Bricks] = 10; // Уже есть кирпичи

            var workshop = new IndustrialBuilding.Workshop(
                building,
                NaturalResourceType.Clay,
                ConstructionMaterialType.Bricks,
                8
            );

            // Act
            var result = workshop.Process();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(8, building.MaterialsBank[NaturalResourceType.Clay]); // 16 - 8 = 8
            Assert.AreEqual(18, building.ProductsBank[ConstructionMaterialType.Bricks]); // 10 + 8 = 18
        }
    }
}
