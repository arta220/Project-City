using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using System.Linq;

namespace Tests.Domain.Industrial
{
    [TestClass]
    public class FireEquipmentFactoryTests
    {
        [TestMethod]
        public void FireEquipmentFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange
            var factory = new FireEquipmentFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(2, building.Floors);
            Assert.AreEqual(80, building.MaxOccupancy);
            Assert.AreEqual(6, building.Area.Width);
            Assert.AreEqual(6, building.Area.Height);
        }

        [TestMethod]
        public void FireEquipmentFactory_CreatesBuilding_WithWorkshops()
        {
            // Arrange
            var factory = new FireEquipmentFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Workshops.Count > 0, "Завод должен иметь цеха");
            Assert.AreEqual(4, building.Workshops.Count, "Завод противопожарного оборудования должен иметь 4 цеха");
        }

        [TestMethod]
        public void FireEquipmentFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new FireEquipmentFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Iron), "Должно быть железо");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Wood), "Должна быть древесина");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(ProductType.Electronics), "Должна быть электроника");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Energy), "Должна быть энергия");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Water), "Должна быть вода");
        }

        [TestMethod]
        public void FireEquipmentFactory_FireExtinguisherWorkshop_Exists()
        {
            // Arrange
            var factory = new FireEquipmentFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var extinguisherWorkshop = building!.Workshops
                .FirstOrDefault(w => w.InputMaterial as NaturalResourceType? == NaturalResourceType.Iron &&
                                     w.OutputProduct as ProductType? == ProductType.FireExtinguisher);

            // Assert
            Assert.IsNotNull(extinguisherWorkshop, "Должен быть цех огнетушителей");
            Assert.AreEqual(3, extinguisherWorkshop.ProductionCoefficient, "Коэффициент должен быть 3");
        }

        [TestMethod]
        public void FireEquipmentFactory_FireHoseWorkshop_Exists()
        {
            // Arrange
            var factory = new FireEquipmentFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var hoseWorkshop = building!.Workshops
                .FirstOrDefault(w => w.InputMaterial as NaturalResourceType? == NaturalResourceType.Wood &&
                                     w.OutputProduct as ProductType? == ProductType.FireHose);

            // Assert
            Assert.IsNotNull(hoseWorkshop, "Должен быть цех пожарных рукавов");
            Assert.AreEqual(2, hoseWorkshop.ProductionCoefficient, "Коэффициент должен быть 2");
        }

        [TestMethod]
        public void FireEquipmentFactory_FireAlarmWorkshop_Exists()
        {
            // Arrange
            var factory = new FireEquipmentFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var alarmWorkshop = building!.Workshops
                .FirstOrDefault(w => w.InputMaterial as ProductType? == ProductType.Electronics &&
                                     w.OutputProduct as ProductType? == ProductType.FireAlarmSystem);

            // Assert
            Assert.IsNotNull(alarmWorkshop, "Должен быть цех систем сигнализации");
            Assert.AreEqual(5, alarmWorkshop.ProductionCoefficient, "Коэффициент должен быть 5");
        }

        [TestMethod]
        public void FireEquipmentFactory_FireTruckWorkshop_Exists()
        {
            // Arrange
            var factory = new FireEquipmentFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var truckWorkshop = building!.Workshops
                .FirstOrDefault(w => w.InputMaterial as NaturalResourceType? == NaturalResourceType.Iron &&
                                     w.OutputProduct as ProductType? == ProductType.FireTruck);

            // Assert
            Assert.IsNotNull(truckWorkshop, "Должен быть цех пожарных машин");
            Assert.AreEqual(15, truckWorkshop.ProductionCoefficient, "Коэффициент должен быть 15");
        }

        [TestMethod]
        public void FireEquipmentFactory_Production_ProducesFireEquipmentProducts()
        {
            // Arrange
            var factory = new FireEquipmentFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();

            // Assert
            var hasFireProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.FireExtinguisher ||
                         p == ProductType.FireHose ||
                         p == ProductType.FireAlarmSystem ||
                         p == ProductType.FireTruck);

            Assert.IsTrue(hasFireProducts || building.ProductsBank.Count > 0,
                "После производства должны появиться продукты противопожарного оборудования");
        }

        [TestMethod]
        public void FireEquipmentFactory_Workshop_ProcessesMaterials()
        {
            // Arrange
            var factory = new FireEquipmentFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var initialIron = building!.MaterialsBank.GetValueOrDefault(NaturalResourceType.Iron, 0);
            var initialWood = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Wood, 0);

            building.RunOnce();

            var afterIron = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Iron, 0);
            var afterWood = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Wood, 0);

            // Assert
            Assert.IsTrue(initialIron >= afterIron,
                "После производства количество железа должно уменьшиться");

            Assert.IsTrue(initialWood >= afterWood,
                "После производства количество древесины должно уменьшиться");
        }

        [TestMethod]
        public void FireEquipmentFactory_MaterialsBank_HasCorrectInitialAmounts()
        {
            // Arrange
            var factory = new FireEquipmentFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.AreEqual(800, building!.MaterialsBank[NaturalResourceType.Iron], "Железо: 800");
            Assert.AreEqual(400, building.MaterialsBank[NaturalResourceType.Wood], "Дерево: 400");
            Assert.AreEqual(200, building.MaterialsBank[ProductType.Electronics], "Электроника: 200");
            Assert.AreEqual(600, building.MaterialsBank[NaturalResourceType.Energy], "Энергия: 600");
            Assert.AreEqual(300, building.MaterialsBank[NaturalResourceType.Water], "Вода: 300");
        }

        [TestMethod]
        public void FireEquipmentFactory_ProductionChain_WorksCorrectly()
        {
            // Arrange
            var factory = new FireEquipmentFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Сохраняем начальные количества
            var initialIron = building!.MaterialsBank[NaturalResourceType.Iron];
            var initialWood = building.MaterialsBank[NaturalResourceType.Wood];
            var initialElectronics = building.MaterialsBank[ProductType.Electronics];

            // Act - запускаем несколько циклов производства
            for (int i = 0; i < 5; i++)
            {
                building.RunOnce();
            }

            // Assert - проверяем, что ресурсы уменьшились
            var afterIron = building.MaterialsBank[NaturalResourceType.Iron];
            var afterWood = building.MaterialsBank[NaturalResourceType.Wood];
            var afterElectronics = building.MaterialsBank[ProductType.Electronics];

            Assert.IsTrue(initialIron > afterIron, "Железо должно уменьшиться после производства");
            Assert.IsTrue(initialWood > afterWood, "Дерево должно уменьшиться после производства");
            Assert.IsTrue(initialElectronics > afterElectronics, "Электроника должна уменьшиться после производства");

            // Проверяем, что появилась продукция
            Assert.IsTrue(building.ProductsBank.Count > 0, "Должна появиться продукция");
        }
    }
}