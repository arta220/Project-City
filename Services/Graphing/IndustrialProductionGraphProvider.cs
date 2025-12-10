using OxyPlot;
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
                Title = "Производство картона",
                Color = OxyColors.Blue
            };

            // Линия использованных материалов
            var materialsLine = new LineSeries
            {
                Title = "Материалы",
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
                Title = "Производство упаковки",
                Color = OxyColors.Green
            };

            // Линия использованных материалов
            var materialsLine = new LineSeries
            {
                Title = "Материалы",
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
    /// Провайдер графика для производства косметики
    /// </summary>
    public class CosmeticsProductionGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Производство косметики";
        public string GraphTitle => "Статистика производства косметики";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public CosmeticsProductionGraphProvider(IIndustrialProductionService productionService)
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

            // Линия производства косметики
            var productionLine = new LineSeries
            {
                Title = "Производство косметики",
                Color = OxyColors.Purple
            };

            // Линия использованных материалов
            var materialsLine = new LineSeries
            {
                Title = "Материалы",
                Color = OxyColors.Pink
            };

            // Заполнение данными
            foreach (var dataPoint in statistics.CosmeticsHistory)
            {
                productionLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.CosmeticsProduction));
                materialsLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.CosmeticsMaterialsUsed));
            }

            plotModel.Series.Add(productionLine);
            plotModel.Series.Add(materialsLine);

            return plotModel;
        }
    }
}

