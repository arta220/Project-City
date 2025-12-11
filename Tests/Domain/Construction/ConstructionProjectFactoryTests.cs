using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Construction;
using Domain.Common.Enums;
using Domain.Factories;
using System.Collections.Generic;

namespace Tests.Domain.Construction
{
    [TestClass]
    public class ConstructionProjectFactoryTests
    {
        private ConstructionProjectFactory _factory = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _factory = new ConstructionProjectFactory();
        }

        [TestMethod]
        public void CreateProject_ReturnsSmallHouseProject_ForSmallHouseFactory()
        {
            // Act
            var project = _factory.CreateProject(new SmallHouseFactory());

            // Assert
            Assert.IsNotNull(project);
            Assert.IsInstanceOfType(project.TargetBuildingFactory, typeof(SmallHouseFactory));

            // Проверяем требуемые материалы для маленького дома
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(ConstructionMaterialType.Bricks));
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(ConstructionMaterialType.Cement));
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(NaturalResourceType.Wood));
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(NaturalResourceType.Glass));

            Assert.AreEqual(100, project.RequiredMaterials[ConstructionMaterialType.Bricks]);
            Assert.AreEqual(50, project.RequiredMaterials[ConstructionMaterialType.Cement]);
            Assert.AreEqual(30, project.RequiredMaterials[NaturalResourceType.Wood]);
            Assert.AreEqual(10, project.RequiredMaterials[NaturalResourceType.Glass]);

            Assert.AreEqual(1, project.ConstructionSpeed);
            Assert.AreEqual(2, project.MinWorkersRequired);
            Assert.AreEqual(200, project.TotalTicksRequired);
        }

        [TestMethod]
        public void CreateProject_ReturnsApartmentProject_ForApartmentFactory()
        {
            // Act
            var project = _factory.CreateProject(new ApartmentFactory());

            // Assert
            Assert.IsNotNull(project);
            Assert.IsInstanceOfType(project.TargetBuildingFactory, typeof(ApartmentFactory));

            // Проверяем требуемые материалы для многоквартирного дома
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(ConstructionMaterialType.Concrete));
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(ConstructionMaterialType.Rebar));
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(ConstructionMaterialType.Bricks));
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(NaturalResourceType.Glass));
            Assert.IsTrue(project.RequiredMaterials.ContainsKey(ProductType.Steel));

            Assert.AreEqual(500, project.RequiredMaterials[ConstructionMaterialType.Concrete]);
            Assert.AreEqual(200, project.RequiredMaterials[ConstructionMaterialType.Rebar]);
            Assert.AreEqual(300, project.RequiredMaterials[ConstructionMaterialType.Bricks]);
            Assert.AreEqual(50, project.RequiredMaterials[NaturalResourceType.Glass]);
            Assert.AreEqual(100, project.RequiredMaterials[ProductType.Steel]);

            Assert.AreEqual(1, project.ConstructionSpeed);
            Assert.AreEqual(5, project.MinWorkersRequired);
            Assert.AreEqual(500, project.TotalTicksRequired);
        }

        [TestMethod]
        public void CreateProject_ReturnsNull_ForIndustrialBuildingFactories()
        {
            // Act & Assert
            Assert.IsNull(_factory.CreateProject(new FactoryBuildingFactory()));
            Assert.IsNull(_factory.CreateProject(new WarehouseFactory()));
            Assert.IsNull(_factory.CreateProject(new CardboardFactory()));
            Assert.IsNull(_factory.CreateProject(new PackagingFactory()));
            Assert.IsNull(_factory.CreateProject(new PharmaceuticalFactoryFactory()));
            Assert.IsNull(_factory.CreateProject(new RecyclingPlantFactoryFactory()));
            Assert.IsNull(_factory.CreateProject(new CementFactory()));
            Assert.IsNull(_factory.CreateProject(new BrickFactory()));
            Assert.IsNull(_factory.CreateProject(new ConcreteFactory()));
            Assert.IsNull(_factory.CreateProject(new ReinforcedConcreteFactory()));
        }

        [TestMethod]
        public void CreateProject_ReturnsNull_ForUnsupportedFactory()
        {
            // Arrange
            var unsupportedFactory = new UtilityOfficeFactory(); // Фабрика, для которой не определен проект

            // Act
            var project = _factory.CreateProject(unsupportedFactory);

            // Assert
            Assert.IsNull(project);
        }

        [TestMethod]
        public void CreateProject_ReturnsNull_ForNullFactory()
        {
            // Act
            var project = _factory.CreateProject(null!);

            // Assert
            Assert.IsNull(project);
        }
    }
}
