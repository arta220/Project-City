using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Graphing;
using Services.IndustrialProduction;
using System.Collections.Generic;
using Domain.Common.Time;

namespace Tests.Services.Graphing
{
    [TestClass]
    public class FireEquipmentGraphProviderTests
    {
        private class TestProductionService : IIndustrialProductionService
        {
            private readonly IndustrialProductionStatistics _statistics;

            public TestProductionService(IndustrialProductionStatistics statistics)
            {
                _statistics = statistics;
            }

            public TestProductionService() : this(CreateDefaultStatistics())
            {
            }

            private static IndustrialProductionStatistics CreateDefaultStatistics()
            {
                return new IndustrialProductionStatistics
                {
                    FireEquipmentHistory = new List<ProductionDataPoint>
                    {
                        new ProductionDataPoint(
                            tick: 1,
                            cardboardProduction: 0,
                            packagingProduction: 0,
                            cardboardMaterialsUsed: 0,
                            packagingMaterialsUsed: 0,
                            fireEquipmentProduction: 5,
                            fireEquipmentMaterialsUsed: 30,
                            roboticsProduction: 0,
                            roboticsMaterialsUsed: 0
                        ),
                        new ProductionDataPoint(
                            tick: 2,
                            cardboardProduction: 0,
                            packagingProduction: 0,
                            cardboardMaterialsUsed: 0,
                            packagingMaterialsUsed: 0,
                            fireEquipmentProduction: 8,
                            fireEquipmentMaterialsUsed: 45,
                            roboticsProduction: 0,
                            roboticsMaterialsUsed: 0
                        ),
                        new ProductionDataPoint(
                            tick: 3,
                            cardboardProduction: 0,
                            packagingProduction: 0,
                            cardboardMaterialsUsed: 0,
                            packagingMaterialsUsed: 0,
                            fireEquipmentProduction: 12,
                            fireEquipmentMaterialsUsed: 60,
                            roboticsProduction: 0,
                            roboticsMaterialsUsed: 0
                        )
                    }
                };
            }

            public IndustrialProductionStatistics GetStatistics()
            {
                return _statistics;
            }

            public void Update(SimulationTime currentTime)
            {
                // Пустая реализация для тестов
            }
        }

        [TestMethod]
        public void FireEquipmentGraphProvider_CreatesPlotModel_Successfully()
        {
            // Arrange
            var testService = new TestProductionService();
            var provider = new FireEquipmentGraphProvider(testService);

            // Act
            var plotModel = provider.CreatePlotModel();

            // Assert
            Assert.IsNotNull(plotModel);
            Assert.AreEqual("Производство противопожарного оборудования", plotModel.Title);
        }

        [TestMethod]
        public void FireEquipmentGraphProvider_HasCorrectProperties()
        {
            // Arrange
            var testService = new TestProductionService();
            var provider = new FireEquipmentGraphProvider(testService);

            // Assert
            Assert.AreEqual("Противопожарное оборудование", provider.SystemName);
            Assert.AreEqual("Производство противопожарного оборудования", provider.GraphTitle);
            Assert.AreEqual("Время (тики)", provider.XAxisTitle);
            Assert.AreEqual("Количество", provider.YAxisTitle);
        }

        [TestMethod]
        public void FireEquipmentGraphProvider_CreatesFiveSeries()
        {
            // Arrange
            var testService = new TestProductionService();
            var provider = new FireEquipmentGraphProvider(testService);

            // Act
            var plotModel = provider.CreatePlotModel();

            // Assert
            Assert.AreEqual(5, plotModel.Series.Count, "Должно быть 5 серий данных");
        }

        [TestMethod]
        public void BothGraphProviders_WorkWithDifferentServices()
        {
            // Arrange - разные сервисы с разными данными
            var roboticsStatistics = new IndustrialProductionStatistics
            {
                RoboticsHistory = new List<ProductionDataPoint>
                {
                    new ProductionDataPoint(1, 0, 0, 0, 0, 0, 0, 10, 50)
                }
            };

            var fireStatistics = new IndustrialProductionStatistics
            {
                FireEquipmentHistory = new List<ProductionDataPoint>
                {
                    new ProductionDataPoint(1, 0, 0, 0, 0, 5, 30, 0, 0)
                }
            };

            var roboticsService = new TestProductionService(roboticsStatistics);
            var fireService = new TestProductionService(fireStatistics);

            var roboticsProvider = new RoboticsGraphProvider(roboticsService);
            var fireProvider = new FireEquipmentGraphProvider(fireService);

            // Act
            var roboticsPlot = roboticsProvider.CreatePlotModel();
            var firePlot = fireProvider.CreatePlotModel();

            // Assert
            Assert.IsNotNull(roboticsPlot);
            Assert.IsNotNull(firePlot);
            Assert.AreNotEqual(roboticsPlot.Title, firePlot.Title, "Заголовки должны отличаться");
        }

        [TestMethod]
        public void FireEquipmentGraphProvider_SeriesContainDataPoints()
        {
            // Arrange
            var testService = new TestProductionService();
            var provider = new FireEquipmentGraphProvider(testService);

            // Act
            var plotModel = provider.CreatePlotModel();

            // Assert - проверяем что серии содержат точки данных
            int seriesWithData = 0;
            foreach (var series in plotModel.Series)
            {
                if (series is OxyPlot.Series.LineSeries lineSeries)
                {
                    if (lineSeries.Points.Count > 0)
                    {
                        seriesWithData++;
                    }
                }
            }

            Assert.IsTrue(seriesWithData >= 1,
                "Хотя бы одна серия должна содержать точки данных");
        }

        [TestMethod]
        public void FireEquipmentGraphProvider_EmptyHistory_CreatesEmptyPlot()
        {
            // Arrange - сервис с пустой историей
            var emptyStatistics = new IndustrialProductionStatistics
            {
                FireEquipmentHistory = new List<ProductionDataPoint>() // Пустой список
            };

            var testService = new TestProductionService(emptyStatistics);
            var provider = new FireEquipmentGraphProvider(testService);

            // Act
            var plotModel = provider.CreatePlotModel();

            // Assert
            Assert.IsNotNull(plotModel, "График должен создаваться даже при пустых данных");
            Assert.AreEqual("Производство противопожарного оборудования", plotModel.Title);

            // Проверяем что оси созданы
            Assert.IsTrue(plotModel.Axes.Count >= 2, "Должны быть созданы оси X и Y");
        }
    }
}