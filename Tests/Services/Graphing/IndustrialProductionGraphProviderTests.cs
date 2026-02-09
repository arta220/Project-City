//using Domain.Buildings;
//using Domain.Common.Base;
//using Domain.Common.Enums;
//using Domain.Common.Time;
//using Domain.Factories;
//using Domain.Map;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Services.BuildingRegistry;
//using Services.Graphing;
//using Services.IndustrialProduction;
//using System.Collections.Generic;
//using System.Linq;

//namespace Tests.Services.Graphing
//{
//    [TestClass]
//    public class IndustrialProductionGraphProviderTests
//    {
//        [TestMethod]
//        public void CardboardProductionGraphProvider_CreatesPlotModel()
//        {
//            // Arrange
//            var mockRegistry = new MockBuildingRegistry();
//            var cardboardFactory = new CardboardFactory();
//            mockRegistry.AddBuilding(cardboardFactory.Create() as IndustrialBuilding);

//            var productionService = new IndustrialProductionService(mockRegistry);
//            var provider = new CardboardProductionGraphProvider(productionService);

//            // Добавляем данные
//            productionService.Update(new SimulationTime(1));
//            productionService.Update(new SimulationTime(2));

//            // Act
//            var plotModel = provider.CreatePlotModel();

//            // Assert
//            Assert.IsNotNull(plotModel);
//            Assert.AreEqual("Статистика производства картона", plotModel.Title);
//            Assert.IsTrue(plotModel.Axes.Count >= 2, "Должны быть оси X и Y");
//            Assert.IsTrue(plotModel.Series.Count > 0, "Должны быть серии данных");
//        }

//        [TestMethod]
//        public void PackagingProductionGraphProvider_CreatesPlotModel()
//        {
//            // Arrange
//            var mockRegistry = new MockBuildingRegistry();
//            var packagingFactory = new PackagingFactory();
//            mockRegistry.AddBuilding(packagingFactory.Create() as IndustrialBuilding);

//            var productionService = new IndustrialProductionService(mockRegistry);
//            var provider = new PackagingProductionGraphProvider(productionService);

//            // Добавляем данные
//            productionService.Update(new SimulationTime(1));
//            productionService.Update(new SimulationTime(2));

//            // Act
//            var plotModel = provider.CreatePlotModel();

//            // Assert
//            Assert.IsNotNull(plotModel);
//            Assert.AreEqual("Статистика производства упаковки", plotModel.Title);
//            Assert.IsTrue(plotModel.Axes.Count >= 2, "Должны быть оси X и Y");
//            Assert.IsTrue(plotModel.Series.Count > 0, "Должны быть серии данных");
//        }

//        [TestMethod]
//        public void GraphProviders_HaveCorrectSystemNames()
//        {
//            // Arrange
//            var mockRegistry = new MockBuildingRegistry();
//            var productionService = new IndustrialProductionService(mockRegistry);
//            var cardboardProvider = new CardboardProductionGraphProvider(productionService);
//            var packagingProvider = new PackagingProductionGraphProvider(productionService);

//            // Assert
//            Assert.AreEqual("Производство картона", cardboardProvider.SystemName);
//            Assert.AreEqual("Производство упаковки", packagingProvider.SystemName);
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

