using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Services.Interfaces;
using Services.JewelryProduction;

namespace Services.Graphing
{
    public class JewelryGraphProvider : IGraphDataProvider
    {
        private readonly JewelryProductionService _jewelryService;

        public string SystemName => "Ювелирно производство";
        public string GraphTitle => "Статистика ювелирного производства";
        public string XAxisTitle => "Время (тики симуляции)";
        public string YAxisTitle => "Количество / Доход";

        public JewelryGraphProvider(JewelryProductionService jewelryService)
        {
            _jewelryService = jewelryService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };

            // Ось X - время
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = XAxisTitle
            });

            // Ось Y1 - количество
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Количество произведенных изделий",
                Key = "QuantityAxis"
            });

            // Ось Y2 - доход
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Доход (деньги)",
                Key = "RevenueAxis"
            });

            // График производства
            var productionSeries = new LineSeries
            {
                Title = "Производство (шт)",
                YAxisKey = "QuantityAxis"
            };

            // График дохода
            var revenueSeries = new LineSeries
            {
                Title = "Доход (деньги)",
                YAxisKey = "RevenueAxis",
                Color = OxyColors.Red
            };

            // Заполняем данными из сервиса
            var productionData = _jewelryService.GlobalProductionHistory;
            var revenueData = _jewelryService.GlobalRevenueHistory;

            for (int i = 0; i < productionData.Count; i++)
            {
                productionSeries.Points.Add(new DataPoint(i, productionData[i]));
            }

            for (int i = 0; i < revenueData.Count; i++)
            {
                revenueSeries.Points.Add(new DataPoint(i, (double)revenueData[i]));
            }

            plotModel.Series.Add(productionSeries);
            plotModel.Series.Add(revenueSeries);

            return plotModel;
        }
    }
}