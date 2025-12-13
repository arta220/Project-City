using OxyPlot;
using OxyPlot.Series;
using Services.IndustrialProduction;

namespace Services.Graphing
{
    /// <summary>
    /// Провайдер графика для химического производства
    /// </summary>
    public class ChemicalProductionGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Химическое производство";
        public string GraphTitle => "Статистика химического производства";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public ChemicalProductionGraphProvider(IIndustrialProductionService productionService)
        {
            _productionService = productionService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _productionService.GetStatistics();

            // Добавление осей
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle
            });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = YAxisTitle
            });

            // График производства - используем существующие данные о картоне и упаковке
            // Предполагаем, что химическое производство отображается через аналогичные метрики

            var productionLine = new LineSeries
            {
                Title = "Химическое производство (условные единицы)",
                Color = OxyColors.Blue
            };

            var materialsLine = new LineSeries
            {
                Title = "Использовано сырья для химии",
                Color = OxyColors.Orange
            };

            // Адаптируем под существующие данные
            // Предполагаем, что chemicalData содержится в CardboardHistory или PackagingHistory
            int tick = 0;
            if (statistics.CardboardHistory != null)
            {
                foreach (var dataPoint in statistics.CardboardHistory)
                {
                    // Преобразуем данные картона в данные химического производства
                    // В реальном приложении здесь будет доступ к реальным данным
                    var chemicalProduction = dataPoint.CardboardProduction * 0.5f; // Пример преобразования
                    var chemicalMaterials = dataPoint.CardboardMaterialsUsed * 0.3f;

                    productionLine.Points.Add(new DataPoint(tick, chemicalProduction));
                    materialsLine.Points.Add(new DataPoint(tick, chemicalMaterials));
                    tick++;
                }
            }

            plotModel.Series.Add(productionLine);
            plotModel.Series.Add(materialsLine);

            return plotModel;
        }
    }

    /// <summary>
    /// Провайдер графика для логистики
    /// </summary>
    public class LogisticsGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Логистика";
        public string GraphTitle => "Статистика логистики";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public LogisticsGraphProvider(IIndustrialProductionService productionService)
        {
            _productionService = productionService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _productionService.GetStatistics();

            // Добавление осей
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle
            });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = YAxisTitle
            });

            // Графики логистики - используем существующие данные
            var incomingLine = new LineSeries
            {
                Title = "Поступления на склад",
                Color = OxyColors.Green
            };

            var outgoingLine = new LineSeries
            {
                Title = "Отгрузки со склада",
                Color = OxyColors.Red
            };

            var efficiencyLine = new LineSeries
            {
                Title = "Эффективность логистики",
                Color = OxyColors.Purple,
                LineStyle = LineStyle.Dash
            };

            // Адаптируем под существующие данные
            int tick = 0;
            if (statistics.PackagingHistory != null)
            {
                foreach (var dataPoint in statistics.PackagingHistory)
                {
                    // Преобразуем данные упаковки в данные логистики
                    // В реальном приложении здесь будет доступ к реальным данным
                    var incoming = dataPoint.PackagingProduction * 0.7f;
                    var outgoing = dataPoint.PackagingProduction * 0.6f;
                    var efficiency = 70 + (tick % 30); // Пример данных

                    incomingLine.Points.Add(new DataPoint(tick, incoming));
                    outgoingLine.Points.Add(new DataPoint(tick, outgoing));
                    efficiencyLine.Points.Add(new DataPoint(tick, efficiency));
                    tick++;
                }
            }

            plotModel.Series.Add(incomingLine);
            plotModel.Series.Add(outgoingLine);
            plotModel.Series.Add(efficiencyLine);

            return plotModel;
        }
    }

    /// <summary>
    /// Комбинированный график для химии и логистики
    /// </summary>
    public class CombinedChemicalLogisticsGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Химия и Логистика";
        public string GraphTitle => "Комбинированная статистика";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public CombinedChemicalLogisticsGraphProvider(IIndustrialProductionService productionService)
        {
            _productionService = productionService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _productionService.GetStatistics();

            // Добавление осей
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle
            });

            // Левая ось для производства
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Производство",
                Key = "ProductionAxis"
            });

            // Правая ось для эффективности
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Right,
                Title = "Эффективность (%)",
                Key = "EfficiencyAxis",
                Minimum = 0,
                Maximum = 100
            });

            // Химическое производство
            var chemicalProductionLine = new LineSeries
            {
                Title = "Хим. производство",
                Color = OxyColors.Blue,
                YAxisKey = "ProductionAxis"
            };

            // Логистические операции
            var logisticsOperationsLine = new LineSeries
            {
                Title = "Логистика (операции)",
                Color = OxyColors.Green,
                YAxisKey = "ProductionAxis"
            };

            // Эффективность
            var combinedEfficiencyLine = new LineSeries
            {
                Title = "Общая эффективность",
                Color = OxyColors.Purple,
                YAxisKey = "EfficiencyAxis",
                LineStyle = LineStyle.Dash
            };

            // Загрязнение (если данные доступны)
            var pollutionLine = new LineSeries
            {
                Title = "Уровень загрязнения",
                Color = OxyColors.Brown,
                YAxisKey = "ProductionAxis",
                LineStyle = LineStyle.Dot
            };

            // Заполнение данными
            int tick = 0;
            if (statistics.CardboardHistory != null && statistics.PackagingHistory != null)
            {
                // Используем данные из обоих источников
                var minLength = Math.Min(
                    statistics.CardboardHistory.Count,
                    statistics.PackagingHistory.Count);

                for (int i = 0; i < minLength; i++)
                {
                    var cardboardData = statistics.CardboardHistory[i];
                    var packagingData = statistics.PackagingHistory[i];

                    // Расчет производных данных
                    var chemicalProd = cardboardData.CardboardProduction * 0.8f + packagingData.PackagingProduction * 0.2f;
                    var logisticsOps = (cardboardData.CardboardMaterialsUsed + packagingData.PackagingMaterialsUsed) * 0.5f;
                    var efficiency = 75 + (tick % 25); // Пример расчета эффективности
                    var pollution = 20 + (tick % 15); // Пример уровня загрязнения

                    chemicalProductionLine.Points.Add(new DataPoint(tick, chemicalProd));
                    logisticsOperationsLine.Points.Add(new DataPoint(tick, logisticsOps));
                    combinedEfficiencyLine.Points.Add(new DataPoint(tick, efficiency));
                    pollutionLine.Points.Add(new DataPoint(tick, pollution));

                    tick++;
                }
            }

            plotModel.Series.Add(chemicalProductionLine);
            plotModel.Series.Add(logisticsOperationsLine);
            plotModel.Series.Add(combinedEfficiencyLine);
            plotModel.Series.Add(pollutionLine);

            return plotModel;
        }
    }

    /// <summary>
    /// График для сравнения разных типов производства
    /// </summary>
    public class ProductionComparisonGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Сравнение производств";
        public string GraphTitle => "Сравнение химии, картона и упаковки";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Производство";

        public ProductionComparisonGraphProvider(IIndustrialProductionService productionService)
        {
            _productionService = productionService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _productionService.GetStatistics();

            // Добавление осей
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle
            });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = YAxisTitle
            });

            // Линии для разных типов производства
            var chemicalLine = new LineSeries
            {
                Title = "Химическое производство",
                Color = OxyColors.Blue
            };

            var cardboardLine = new LineSeries
            {
                Title = "Производство картона",
                Color = OxyColors.Green
            };

            var packagingLine = new LineSeries
            {
                Title = "Производство упаковки",
                Color = OxyColors.Orange
            };

            // Заполнение данными
            int tick = 0;
            if (statistics.CardboardHistory != null && statistics.PackagingHistory != null)
            {
                var minLength = Math.Min(
                    statistics.CardboardHistory.Count,
                    statistics.PackagingHistory.Count);

                for (int i = 0; i < minLength; i++)
                {
                    var cardboardData = statistics.CardboardHistory[i];
                    var packagingData = statistics.PackagingHistory[i];

                    // Расчет производных данных
                    var chemicalProduction = (cardboardData.CardboardProduction + packagingData.PackagingProduction) * 0.6f;

                    chemicalLine.Points.Add(new DataPoint(tick, chemicalProduction));
                    cardboardLine.Points.Add(new DataPoint(tick, cardboardData.CardboardProduction));
                    packagingLine.Points.Add(new DataPoint(tick, packagingData.PackagingProduction));

                    tick++;
                }
            }

            plotModel.Series.Add(chemicalLine);
            plotModel.Series.Add(cardboardLine);
            plotModel.Series.Add(packagingLine);

            return plotModel;
        }
    }
}