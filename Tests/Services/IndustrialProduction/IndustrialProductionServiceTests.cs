//using Domain.Buildings;
//using Domain.Common.Base;
//using Domain.Common.Enums;
//using Domain.Common.Time;
//using Domain.Factories;
//using Domain.Map;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Services.BuildingRegistry;
//using Services.IndustrialProduction;
//using System.Collections.Generic;
//using System.Linq;

//namespace Tests.Services.IndustrialProduction
//{
//    [TestClass]
//    public class IndustrialProductionServiceTests
//    {
//        [TestMethod]
//        public void IndustrialProductionService_TracksCardboardProduction()
//        {
//            // Arrange
//            var mockRegistry = new MockBuildingRegistry();
//            var cardboardFactory = new CardboardFactory();
//            var building = cardboardFactory.Create() as IndustrialBuilding;
//            mockRegistry.AddBuilding(building!);

//            var service = new IndustrialProductionService(mockRegistry);
//            var time = new SimulationTime(1);

//            // Act
//            service.Update(time);
//            var statistics = service.GetStatistics();

//            // Assert
//            Assert.IsNotNull(statistics);
//            Assert.IsTrue(statistics.CardboardHistory.Count > 0, "Должна быть записана история производства картона");
            
//            var lastDataPoint = statistics.CardboardHistory.Last();
//            Assert.AreEqual(time.TotalTicks, lastDataPoint.Tick);
//        }

//        [TestMethod]
//        public void IndustrialProductionService_TracksPackagingProduction()
//        {
//            // Arrange
//            var mockRegistry = new MockBuildingRegistry();
//            var packagingFactory = new PackagingFactory();
//            var building = packagingFactory.Create() as IndustrialBuilding;
//            mockRegistry.AddBuilding(building!);

//            var service = new IndustrialProductionService(mockRegistry);
//            var time = new SimulationTime(1);

//            // Act
//            service.Update(time);
//            var statistics = service.GetStatistics();

//            // Assert
//            Assert.IsNotNull(statistics);
//            Assert.IsTrue(statistics.PackagingHistory.Count > 0, "Должна быть записана история производства упаковки");
            
//            var lastDataPoint = statistics.PackagingHistory.Last();
//            Assert.AreEqual(time.TotalTicks, lastDataPoint.Tick);
//        }

//        [TestMethod]
//        public void IndustrialProductionService_TracksMultipleBuildings()
//        {
//            // Arrange
//            var mockRegistry = new MockBuildingRegistry();
//            var cardboardFactory = new CardboardFactory();
//            var packagingFactory = new PackagingFactory();
            
//            mockRegistry.AddBuilding(cardboardFactory.Create() as IndustrialBuilding);
//            mockRegistry.AddBuilding(packagingFactory.Create() as IndustrialBuilding);

//            var service = new IndustrialProductionService(mockRegistry);
//            var time = new SimulationTime(1);

//            // Act
//            service.Update(time);
//            var statistics = service.GetStatistics();

//            // Assert
//            Assert.IsNotNull(statistics);
//            var lastCardboard = statistics.CardboardHistory.Last();
//            var lastPackaging = statistics.PackagingHistory.Last();
            
//            Assert.IsTrue(lastCardboard.CardboardProduction >= 0 || lastPackaging.PackagingProduction >= 0,
//                "Должна отслеживаться статистика обоих заводов");
//        }

//        [TestMethod]
//        public void IndustrialProductionService_UpdatesOverTime()
//        {
//            // Arrange
//            var mockRegistry = new MockBuildingRegistry();
//            var cardboardFactory = new CardboardFactory();
//            mockRegistry.AddBuilding(cardboardFactory.Create() as IndustrialBuilding);

//            var service = new IndustrialProductionService(mockRegistry);

//            // Act - обновляем несколько раз
//            service.Update(new SimulationTime(1));
//            service.Update(new SimulationTime(2));
//            service.Update(new SimulationTime(3));
            
//            var statistics = service.GetStatistics();

//            // Assert
//            Assert.IsTrue(statistics.CardboardHistory.Count >= 3, 
//                "Должна быть история за несколько тиков");
//        }
//    }

//    // Вспомогательный класс для мокирования BuildingRegistry
//    internal class MockBuildingRegistry : IBuildingRegistry
//    {
//        private readonly List<IndustrialBuilding> _buildings = new();

//        public void AddBuilding(IndustrialBuilding building)
//        {
//            _buildings.Add(building);
//        }

//        public IEnumerable<T> GetBuildings<T>()
//        {
//            return _buildings.OfType<T>();
//        }

//        public (Placement? placement, bool found) TryGetPlacement(MapObject building)
//        {
//            return (null, false);
//        }
//    }
//}

