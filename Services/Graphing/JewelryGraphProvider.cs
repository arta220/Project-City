using OxyPlot;
using OxyPlot.Series;
using Services.JewelryProduction;

namespace Services.Graphing
{
    /// <summary>
    /// Провайдер графика для производства ювелирных изделий
    /// </summary>
    public class JewelryGraphProvider : IGraphDataProvider
    {
        private readonly IJewelryProductionService _jewelryService;

        public string SystemName => "Ювелирное производство";
        public string GraphTitle => "Статистика ювелирного производства";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public JewelryGraphProvider(IJewelryProductionService jewelryService)
        {
            _jewelryService = jewelryService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _jewelryService.GetStatistics();

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

            // Линия производства ювелирных изделий
            var productionLine = new LineSeries 
            { 
                Title = "Производство ювелирных изделий", 
                Color = OxyColors.Blue 
            };

            // Линия использованных материалов
            var materialsLine = new LineSeries 
            { 
                Title = "Материалы", 
                Color = OxyColors.Orange 
            };

            // Заполнение данными
            foreach (var dataPoint in statistics.ProductionHistory)
            {
                productionLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.JewelryProduction));
                materialsLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.MaterialsUsed));
            }

            plotModel.Series.Add(productionLine);
            plotModel.Series.Add(materialsLine);

            return plotModel;
        }
    }
}