using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using Services.IndustrialProduction;

namespace Services.Graphing
{
    /// <summary>
    /// Провайдер графика для производства картона
    /// </summary>
    public class CardboardProductionGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Производство картона";
        public string GraphTitle => "Статистика производства картона";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public CardboardProductionGraphProvider(IIndustrialProductionService productionService)
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

            // Линия производства картона
            var productionLine = new LineSeries
            {
                Title = "Производство картона (листы, коробки, упаковка)",
                Color = OxyColors.Blue
            };

            // Линия использованных материалов
            var materialsLine = new LineSeries
            {
                Title = "Использовано материалов (щепа, бумага, химикаты, вода, энергия)",
                Color = OxyColors.Orange
            };

            // Заполнение данными
            foreach (var dataPoint in statistics.CardboardHistory)
            {
                productionLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.CardboardProduction));
                materialsLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.CardboardMaterialsUsed));
            }

            plotModel.Series.Add(productionLine);
            plotModel.Series.Add(materialsLine);

            return plotModel;
        }
    }

    /// <summary>
    /// Провайдер графика для производства упаковки
    /// </summary>
    public class PackagingProductionGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Производство упаковки";
        public string GraphTitle => "Статистика производства упаковки";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public PackagingProductionGraphProvider(IIndustrialProductionService productionService)
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

            // Линия производства упаковки
            var productionLine = new LineSeries
            {
                Title = "Производство упаковки (коробки, бутылки, банки, ящики)",
                Color = OxyColors.Green
            };

            // Линия использованных материалов
            var materialsLine = new LineSeries
            {
                Title = "Использовано материалов (картон, пластик, стекло, алюминий, дерево, краска)",
                Color = OxyColors.Red
            };

            // Заполнение данными
            foreach (var dataPoint in statistics.PackagingHistory)
            {
                productionLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.PackagingProduction));
                materialsLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.PackagingMaterialsUsed));
            }

            plotModel.Series.Add(productionLine);
            plotModel.Series.Add(materialsLine);

            return plotModel;
        }
    }

    /// <summary>
    /// Провайдер графика для добывающего завода
    /// </summary>
    public class ResourceExtractionGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Добывающий завод";
        public string GraphTitle => "Статистика добывающего завода";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public ResourceExtractionGraphProvider(IIndustrialProductionService productionService)
        {
            _productionService = productionService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _productionService.GetStatistics();

            if (!statistics.ResourceExtractionHistory.Any())
            {
                return plotModel; // Нет данных
            }

            // Ось X (время)
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle,
                Minimum = 0,
                MaximumPadding = 0.1,
                MinimumPadding = 0
            });

            // Ось Y (количество) - АВТОМАТИЧЕСКОЕ масштабирование
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = YAxisTitle,
                Minimum = 0,
                Maximum = double.NaN,
                MaximumPadding = 0.1,
                MinimumPadding = 0.05
            });

            // ОДНА линия - только производство
            var productionLine = new LineSeries
            {
                Title = "Производство (железо, дерево, уголь)",
                Color = OxyColors.Brown,
                StrokeThickness = 2
            };

            // Заполнение данными - БЕРЕМ НАКОПЛЕННУЮ ПРОДУКЦИЮ (сумму)
            int accumulatedProduction = 0;
            foreach (var dataPoint in statistics.ResourceExtractionHistory)
            {
                accumulatedProduction += dataPoint.ResourceExtractionProduction;
                productionLine.Points.Add(new DataPoint(dataPoint.Tick, accumulatedProduction));
            }

            plotModel.Series.Add(productionLine);

            // Включаем легенду
            plotModel.IsLegendVisible = true;

            return plotModel;
        }
    }

    /// <summary>
    /// Провайдер графика для деревообрабатывающего завода
    /// </summary>
    public class WoodProcessingGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Деревообрабатывающий завод";
        public string GraphTitle => "Статистика деревообрабатывающего завода";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public WoodProcessingGraphProvider(IIndustrialProductionService productionService)
        {
            _productionService = productionService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _productionService.GetStatistics();

            if (!statistics.WoodProcessingHistory.Any())
            {
                return plotModel;
            }

            // Ось X (время)
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle,
                Minimum = 0,
                MaximumPadding = 0.1,
                MinimumPadding = 0
            });

            // Ось Y (количество)
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = YAxisTitle,
                Minimum = 0,
                Maximum = double.NaN,
                MaximumPadding = 0.1,
                MinimumPadding = 0.05
            });

            // ОДНА линия - только производство
            var productionLine = new LineSeries
            {
                Title = "Производство (пиломатериалы, мебель, бумага, ящики)",
                Color = OxyColors.Green,
                StrokeThickness = 2
            };

            // Заполнение данными - БЕРЕМ НАКОПЛЕННУЮ ПРОДУКЦИЮ
            int accumulatedProduction = 0;
            foreach (var dataPoint in statistics.WoodProcessingHistory)
            {
                accumulatedProduction += dataPoint.WoodProcessingProduction;
                productionLine.Points.Add(new DataPoint(dataPoint.Tick, accumulatedProduction));
            }

            plotModel.Series.Add(productionLine);

            // Включаем легенду
            plotModel.IsLegendVisible = true;

            return plotModel;
        }
    }

    /// <summary>
    /// Провайдер графика для перерабатывающего завода
    /// </summary>
    public class RecyclingProcessingGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Перерабатывающий завод";
        public string GraphTitle => "Статистика перерабатывающего завода";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public RecyclingProcessingGraphProvider(IIndustrialProductionService productionService)
        {
            _productionService = productionService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _productionService.GetStatistics();

            if (!statistics.RecyclingProcessingHistory.Any())
            {
                return plotModel;
            }

            // Ось X (время)
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle,
                Minimum = 0,
                MaximumPadding = 0.1,
                MinimumPadding = 0
            });

            // Ось Y (количество)
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = YAxisTitle,
                Minimum = 0,
                Maximum = double.NaN,
                MaximumPadding = 0.1,
                MinimumPadding = 0.05
            });

            // ОДНА линия - только производство
            var productionLine = new LineSeries
            {
                Title = "Производство (сталь, пластик, топливо, бутылки)",
                Color = OxyColors.Teal,
                StrokeThickness = 2
            };

            // Заполнение данными - БЕРЕМ НАКОПЛЕННУЮ ПРОДУКЦИЮ
            int accumulatedProduction = 0;
            foreach (var dataPoint in statistics.RecyclingProcessingHistory)
            {
                accumulatedProduction += dataPoint.RecyclingProcessingProduction;
                productionLine.Points.Add(new DataPoint(dataPoint.Tick, accumulatedProduction));
            }

            plotModel.Series.Add(productionLine);

            // Включаем легенду
            plotModel.IsLegendVisible = true;

            return plotModel;
        }
    }
}