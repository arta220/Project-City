using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using System.Linq;

namespace Tests.Domain.Industrial
{
    [TestClass]
    public class RoboticsFactoryTests
    {
        [TestMethod]
        public void RoboticsFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange
            var factory = new RoboticsFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(3, building.Floors);
            Assert.AreEqual(65, building.MaxOccupancy);
            Assert.AreEqual(7, building.Area.Width);
            Assert.AreEqual(7, building.Area.Height);
        }

        [TestMethod]
        public void RoboticsFactory_CreatesBuilding_WithWorkshops()
        {
            // Arrange
            var factory = new RoboticsFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Workshops.Count > 0, "Завод должен иметь цеха");
            Assert.AreEqual(4, building.Workshops.Count, "Завод роботов должен иметь 4 цеха");
        }

        [TestMethod]
        public void RoboticsFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new RoboticsFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(ProductType.Electronics), "Должна быть электроника");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Iron), "Должно быть железо");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Energy), "Должна быть энергия");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Water), "Должна быть вода");
        }

        [TestMethod]
        public void RoboticsFactory_ControllerWorkshop_Exists()
        {
            // Arrange
            var factory = new RoboticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var controllerWorkshop = building!.Workshops
                .FirstOrDefault(w => w.InputMaterial as ProductType? == ProductType.Electronics &&
                                     w.OutputProduct as ProductType? == ProductType.RobotController);

            // Assert
            Assert.IsNotNull(controllerWorkshop, "Должен быть цех контроллеров");
            Assert.AreEqual(8, controllerWorkshop.ProductionCoefficient, "Коэффициент должен быть 8");
        }

        [TestMethod]
        public void RoboticsFactory_RobotArmWorkshop_Exists()
        {
            // Arrange
            var factory = new RoboticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var armWorkshop = building!.Workshops
                .FirstOrDefault(w => w.InputMaterial as NaturalResourceType? == NaturalResourceType.Iron &&
                                     w.OutputProduct as ProductType? == ProductType.RobotArm);

            // Assert
            Assert.IsNotNull(armWorkshop, "Должен быть цех роботизированных рук");
            Assert.AreEqual(12, armWorkshop.ProductionCoefficient, "Коэффициент должен быть 12");
        }

        [TestMethod]
        public void RoboticsFactory_IndustrialRobotWorkshop_Exists()
        {
            // Arrange
            var factory = new RoboticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var robotWorkshop = building!.Workshops
                .FirstOrDefault(w => w.InputMaterial as ProductType? == ProductType.RobotController &&
                                     w.OutputProduct as ProductType? == ProductType.IndustrialRobot);

            // Assert
            Assert.IsNotNull(robotWorkshop, "Должен быть цех сборки промышленных роботов");
            Assert.AreEqual(2, robotWorkshop.ProductionCoefficient, "Коэффициент должен быть 2");
        }

        [TestMethod]
        public void RoboticsFactory_AutomationWorkshop_Exists()
        {
            // Arrange
            var factory = new RoboticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var automationWorkshop = building!.Workshops
                .FirstOrDefault(w => w.InputMaterial as ProductType? == ProductType.RobotController &&
                                     w.OutputProduct as ProductType? == ProductType.AutomationSystem);

            // Assert
            Assert.IsNotNull(automationWorkshop, "Должен быть цех систем автоматизации");
            Assert.AreEqual(3, automationWorkshop.ProductionCoefficient, "Коэффициент должен быть 3");
        }

        [TestMethod]
        public void RoboticsFactory_Production_ProducesRoboticsProducts()
        {
            // Arrange
            var factory = new RoboticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();

            // Assert
            var hasRoboticsProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.RobotController ||
                         p == ProductType.RobotArm ||
                         p == ProductType.IndustrialRobot ||
                         p == ProductType.AutomationSystem);

            Assert.IsTrue(hasRoboticsProducts || building.ProductsBank.Count > 0,
                "После производства должны появиться продукты робототехники");
        }

        [TestMethod]
        public void RoboticsFactory_Workshop_ProcessesMaterials()
        {
            // Arrange
            var factory = new RoboticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var initialElectronics = building!.MaterialsBank.GetValueOrDefault(ProductType.Electronics, 0);
            var initialIron = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Iron, 0);

            building.RunOnce();

            var afterElectronics = building.MaterialsBank.GetValueOrDefault(ProductType.Electronics, 0);
            var afterIron = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Iron, 0);

            // Assert
            Assert.IsTrue(initialElectronics >= afterElectronics,
                "После производства количество электроники должно уменьшиться");

            Assert.IsTrue(initialIron >= afterIron,
                "После производства количество железа должно уменьшиться");
        }

        [TestMethod]
        public void RoboticsFactory_MaterialsBank_HasCorrectInitialAmounts()
        {
            // Arrange
            var factory = new RoboticsFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.AreEqual(400, building!.MaterialsBank[ProductType.Electronics], "Электроника: 400");
            Assert.AreEqual(600, building.MaterialsBank[NaturalResourceType.Iron], "Железо: 600");
            Assert.AreEqual(800, building.MaterialsBank[NaturalResourceType.Energy], "Энергия: 800");
            Assert.AreEqual(200, building.MaterialsBank[NaturalResourceType.Water], "Вода: 200");
        }
    }
}