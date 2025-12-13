using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Graphing;
using Services.IndustrialProduction;
using System.Collections.Generic;
using Domain.Common.Time;

namespace Tests.Services.Graphing
{
    [TestClass]
    public class RoboticsGraphProviderTests
    {
        private class TestProductionService : IIndustrialProductionService
        {
            public IndustrialProductionStatistics GetStatistics()
            {
                return new IndustrialProductionStatistics
                {
                    RoboticsHistory = new List<ProductionDataPoint>
                    {
                        new ProductionDataPoint(
                            tick: 1,
                            cardboardProduction: 0,
                            packagingProduction: 0,
                            cardboardMaterialsUsed: 0,
                            packagingMaterialsUsed: 0,
                            fireEquipmentProduction: 0,
                            fireEquipmentMaterialsUsed: 0,
                            roboticsProduction: 10,
                            roboticsMaterialsUsed: 50
                        ),
                        new ProductionDataPoint(
                            tick: 2,
                            cardboardProduction: 0,
                            packagingProduction: 0,
                            cardboardMaterialsUsed: 0,
                            packagingMaterialsUsed: 0,
                            fireEquipmentProduction: 0,
                            fireEquipmentMaterialsUsed: 0,
                            roboticsProduction: 15,
                            roboticsMaterialsUsed: 75
                        ),
                        new ProductionDataPoint(
                            tick: 3,
                            cardboardProduction: 0,
                            packagingProduction: 0,
                            cardboardMaterialsUsed: 0,
                            packagingMaterialsUsed: 0,
                            fireEquipmentProduction: 0,
                            fireEquipmentMaterialsUsed: 0,
                            roboticsProduction: 20,
                            roboticsMaterialsUsed: 100
                        )
                    }
                };
            }

            public void Update(SimulationTime currentTime)
            {
                // Пустая реализация для тестов
            }
        }

        [TestMethod]
        public void RoboticsGraphProvider_CreatesPlotModel_Successfully()
        {
            // Arrange
            var testService = new TestProductionService();
            var provider = new RoboticsGraphProvider(testService);

            // Act
            var plotModel = provider.CreatePlotModel();

            // Assert
            Assert.IsNotNull(plotModel);
            Assert.AreEqual("Производство промышленных роботов", plotModel.Title);
        }

        [TestMethod]
        public void RoboticsGraphProvider_HasCorrectProperties()
        {
            // Arrange
            var testService = new TestProductionService();
            var provider = new RoboticsGraphProvider(testService);

            // Assert
            Assert.AreEqual("Робототехника", provider.SystemName);
            Assert.AreEqual("Производство промышленных роботов", provider.GraphTitle);
            Assert.AreEqual("Время (тики)", provider.XAxisTitle);
            Assert.AreEqual("Количество", provider.YAxisTitle);
        }

        [TestMethod]
        public void RoboticsGraphProvider_CreatesFiveSeries()
        {
            // Arrange
            var testService = new TestProductionService();
            var provider = new RoboticsGraphProvider(testService);

            // Act
            var plotModel = provider.CreatePlotModel();

            // Assert
            Assert.AreEqual(5, plotModel.Series.Count, "Должно быть 5 серий данных");
        }

        [TestMethod]
        public void RoboticsGraphProvider_SeriesContainDataPoints()
        {
            // Arrange
            var testService = new TestProductionService();
            var provider = new RoboticsGraphProvider(testService);

            // Act
            var plotModel = provider.CreatePlotModel();

            // Assert - проверяем что серии содержат точки данных
            foreach (var series in plotModel.Series)
            {
                if (series is OxyPlot.Series.LineSeries lineSeries)
                {
                    Assert.IsTrue(lineSeries.Points.Count >= 3,
                        $"Серия '{lineSeries.Title}' должна содержать точки данных");
                }
            }
        }
    }
}