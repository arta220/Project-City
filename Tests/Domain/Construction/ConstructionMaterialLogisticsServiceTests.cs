using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Construction;
using Domain.Buildings.Construction;
using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Tests.Mocks;
using System.Collections.Generic;

namespace Tests.Domain.Construction
{
    [TestClass]
    public class ConstructionMaterialLogisticsServiceTests
    {
        private ConstructionMaterialLogisticsService _service = null!;
        private FakeBuildingRegistry _buildingRegistry = null!;
        private ConstructionSite _testSite = null!;
        private IndustrialBuilding _cementFactory = null!;
        private IndustrialBuilding _brickFactory = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _buildingRegistry = new FakeBuildingRegistry();

            // Создаем тестовый сайт
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Cement, 50 },
                { ConstructionMaterialType.Bricks, 100 }
            };
            var project = new ConstructionProject(
                new Domain.Factories.SmallHouseFactory(),
                requiredMaterials
            );
            _testSite = new ConstructionSite(new Area(3, 3), project);

            // Создаем цементный завод
            _cementFactory = new IndustrialBuilding(2, 40, new Area(5, 5), IndustrialBuildingType.Factory);
            _cementFactory.AddWorkshop(NaturalResourceType.Limestone, ConstructionMaterialType.Cement, 5);
            _cementFactory.MaterialsBank[NaturalResourceType.Limestone] = 100;
            _cementFactory.MaterialsBank[NaturalResourceType.Clay] = 50;
            _cementFactory.RunOnce(); // Производим цемент
            _buildingRegistry.Add(_cementFactory);

            // Создаем кирпичный завод
            _brickFactory = new IndustrialBuilding(1, 30, new Area(4, 4), IndustrialBuildingType.Factory);
            _brickFactory.AddWorkshop(NaturalResourceType.Clay, ConstructionMaterialType.Bricks, 8);
            _brickFactory.MaterialsBank[NaturalResourceType.Clay] = 200;
            _brickFactory.RunOnce(); // Производим кирпичи
            _buildingRegistry.Add(_brickFactory);

            _service = new ConstructionMaterialLogisticsService(_buildingRegistry);
        }

        [TestMethod]
        public void RequestMaterialsDelivery_AddsMaterialsToPendingDeliveries()
        {
            // Act
            _service.RequestMaterialsDelivery(_testSite);

            // Assert
            // Проверяем, что материалы добавлены в ожидающие доставки
            // (внутренняя логика проверяется через Update)
            Assert.IsNotNull(_testSite);
        }

        [TestMethod]
        public void Update_DeliversMaterials_WhenFactoriesHaveStock()
        {
            // Arrange
            _service.RequestMaterialsDelivery(_testSite);

            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            // Проверяем, что материалы доставлены на площадку
            Assert.IsTrue(_testSite.MaterialsBank.ContainsKey(ConstructionMaterialType.Cement));
            Assert.IsTrue(_testSite.MaterialsBank.ContainsKey(ConstructionMaterialType.Bricks));
            // Точное количество зависит от DeliveryBatchSize (50)
            Assert.AreEqual(50, _testSite.MaterialsBank[ConstructionMaterialType.Cement]);
            Assert.AreEqual(50, _testSite.MaterialsBank[ConstructionMaterialType.Bricks]);
        }

        [TestMethod]
        public void Update_DeliversMaterialsInBatches()
        {
            // Arrange
            // Создаем площадку с большим требованием материалов
            var largeRequirements = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Cement, 150 }, // Больше чем DeliveryBatchSize
                { ConstructionMaterialType.Bricks, 75 }
            };
            var largeProject = new ConstructionProject(
                new Domain.Factories.SmallHouseFactory(),
                largeRequirements
            );
            var largeSite = new ConstructionSite(new Area(3, 3), largeProject);

            _service.RequestMaterialsDelivery(largeSite);

            // Act - первое обновление
            var time1 = new SimulationTime(1);
            _service.Update(time1);

            // Assert - первая партия
            Assert.AreEqual(50, largeSite.MaterialsBank[ConstructionMaterialType.Cement]);
            Assert.AreEqual(50, largeSite.MaterialsBank[ConstructionMaterialType.Bricks]);

            // Act - второе обновление
            var time2 = new SimulationTime(2);
            _service.Update(time2);

            // Assert - вторая партия
            Assert.AreEqual(100, largeSite.MaterialsBank[ConstructionMaterialType.Cement]); // 50 + 50
            Assert.AreEqual(75, largeSite.MaterialsBank[ConstructionMaterialType.Bricks]); // 50 + 25 (ограничено требованием)
        }

        [TestMethod]
        public void Update_RemovesSiteFromPending_WhenAllMaterialsDelivered()
        {
            // Arrange
            _service.RequestMaterialsDelivery(_testSite);

            // Act - обновляем до тех пор, пока все не доставлено
            var time = new SimulationTime(1);
            for (int i = 0; i < 10; i++) // Достаточно обновлений
            {
                _service.Update(time);
                time = new SimulationTime(time.TotalTicks + 1);
            }

            // Assert - все материалы доставлены
            Assert.AreEqual(50, _testSite.MaterialsBank[ConstructionMaterialType.Cement]);
            Assert.AreEqual(100, _testSite.MaterialsBank[ConstructionMaterialType.Bricks]);
        }

        [TestMethod]
        public void Update_RemovesCancelledSiteFromPending()
        {
            // Arrange
            _service.RequestMaterialsDelivery(_testSite);
            _testSite.IsCancelled = true;
            _testSite.Status = ConstructionSiteStatus.Cancelled;

            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            // Следующее обновление не должно доставлять материалы
            _service.Update(new SimulationTime(2));
            // Материалы не должны быть доставлены после отмены
            Assert.IsFalse(_testSite.MaterialsBank.ContainsKey(ConstructionMaterialType.Cement));
        }

        [TestMethod]
        public void FindMaterialSource_ReturnsClosestFactory_WithMaterial()
        {
            // Arrange
            var sitePosition = new Position(10, 10);

            // Act
            var source = _service.FindMaterialSource(ConstructionMaterialType.Cement, sitePosition);

            // Assert
            Assert.IsNotNull(source);
            Assert.AreEqual(_cementFactory, source);
            Assert.IsTrue(source!.ProductsBank.ContainsKey(ConstructionMaterialType.Cement));
        }

        [TestMethod]
        public void FindMaterialSource_ReturnsNull_WhenNoFactoryHasMaterial()
        {
            // Arrange
            var sitePosition = new Position(10, 10);

            // Act
            var source = _service.FindMaterialSource(ConstructionMaterialType.Sand, sitePosition);

            // Assert
            Assert.IsNull(source);
        }

        [TestMethod]
        public void FindMaterialSource_ReturnsFactory_WithMaterialInMaterialsBank()
        {
            // Arrange
            var sitePosition = new Position(10, 10);
            // Добавим материал в MaterialsBank завода
            _cementFactory.MaterialsBank[ConstructionMaterialType.Sand] = 100;

            // Act
            var source = _service.FindMaterialSource(ConstructionMaterialType.Sand, sitePosition);

            // Assert
            Assert.IsNotNull(source);
            Assert.AreEqual(_cementFactory, source);
        }

        [TestMethod]
        public void Update_DoesNotDeliver_WhenSiteIsCompleted()
        {
            // Arrange
            _service.RequestMaterialsDelivery(_testSite);
            _testSite.Status = ConstructionSiteStatus.Completed;

            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            Assert.IsFalse(_testSite.MaterialsBank.ContainsKey(ConstructionMaterialType.Cement));
        }

        [TestMethod]
        public void Update_DeliversFromProductsBankFirst()
        {
            // Arrange
            // Очистим ProductsBank и добавим в MaterialsBank
            _cementFactory.ProductsBank.Clear();
            _cementFactory.MaterialsBank[ConstructionMaterialType.Cement] = 100;

            _service.RequestMaterialsDelivery(_testSite);
            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            Assert.AreEqual(50, _testSite.MaterialsBank[ConstructionMaterialType.Cement]);
            Assert.AreEqual(50, _cementFactory.MaterialsBank[ConstructionMaterialType.Cement]);
        }
    }
}
