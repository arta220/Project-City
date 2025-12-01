using OxyPlot;
using OxyPlot.Series;
using Services.Utilities;

namespace Services.Graphing
{
    public class UtilitiesGraphProvider : IGraphDataProvider
    {
        private readonly IUtilityService _utilityService;

        public string SystemName => "Система ЖКХ";
        public string GraphTitle => "Статистика поломок и починок ЖКХ";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public UtilitiesGraphProvider(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _utilityService.GetStatistics();

            // оси
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

            // ЛИНИИ ПОЧИНОК для каждого типа
            var electricityRepairLine = new LineSeries { Title = "Починки света", Color = OxyColors.LightGreen };
            var gasRepairLine = new LineSeries { Title = "Починки газа", Color = OxyColors.Orange };
            var wasteRepairLine = new LineSeries { Title = "Починки отходов", Color = OxyColors.LightSalmon };
            var waterRepairLine = new LineSeries { Title = "Починки воды", Color = OxyColors.LightBlue };

            // ЛИНИИ ПОЛОМОК 
            var electricityBreakdownLine = new LineSeries { Title = "Поломки света", Color = OxyColors.Yellow };
            var gasBreakdownLine = new LineSeries { Title = "Поломки газа", Color = OxyColors.Red };
            var wasteBreakdownLine = new LineSeries { Title = "Поломки отходов", Color = OxyColors.Brown };
            var waterBreakdownLine = new LineSeries { Title = "Поломки воды", Color = OxyColors.Blue };

            // Заполняем данными
            FillSeriesFromStatistics(
                electricityRepairLine, gasRepairLine, wasteRepairLine, waterRepairLine,
                electricityBreakdownLine, gasBreakdownLine, wasteBreakdownLine, waterBreakdownLine,
                statistics);

            // Добавляем ВСЕ линии в график
            plotModel.Series.Add(electricityRepairLine);
            plotModel.Series.Add(gasRepairLine);
            plotModel.Series.Add(wasteRepairLine);
            plotModel.Series.Add(waterRepairLine);
            plotModel.Series.Add(electricityBreakdownLine);
            plotModel.Series.Add(gasBreakdownLine);
            plotModel.Series.Add(wasteBreakdownLine);
            plotModel.Series.Add(waterBreakdownLine);

            return plotModel;
        }

        private void FillSeriesFromStatistics(
            LineSeries electricityRepairLine, LineSeries gasRepairLine, LineSeries wasteRepairLine, LineSeries waterRepairLine,
            LineSeries electricityBreakdownLine, LineSeries gasBreakdownLine, LineSeries wasteBreakdownLine, LineSeries waterBreakdownLine,
            UtilityStatistics statistics)
        {
            // Данные по ПОЧИНКАМ
            foreach (var dataPoint in statistics.ElectricityHistory)
            {
                electricityRepairLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.RepairCount));
            }
            foreach (var dataPoint in statistics.GasHistory)
            {
                gasRepairLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.RepairCount));
            }
            foreach (var dataPoint in statistics.WasteHistory)
            {
                wasteRepairLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.RepairCount));
            }
            foreach (var dataPoint in statistics.WaterHistory)
            {
                waterRepairLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.RepairCount));
            }

            // Данные по ПОЛОМКАМ
            foreach (var dataPoint in statistics.ElectricityHistory)
            {
                electricityBreakdownLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.BreakdownCount));
            }
            foreach (var dataPoint in statistics.GasHistory)
            {
                gasBreakdownLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.BreakdownCount));
            }
            foreach (var dataPoint in statistics.WasteHistory)
            {
                wasteBreakdownLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.BreakdownCount));
            }
            foreach (var dataPoint in statistics.WaterHistory)
            {
                waterBreakdownLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.BreakdownCount));
            }
        }
    }
}
