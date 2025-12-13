using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Services.IndustrialProduction;

namespace Services.Graphing
{
    /// <summary>
    /// Провайдер графиков для производства промышленных роботов
    /// </summary>
    public class RoboticsGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Робототехника";
        public string GraphTitle => "Производство промышленных роботов";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public RoboticsGraphProvider(IIndustrialProductionService productionService)
        {
            _productionService = productionService;
        }

        public PlotModel CreatePlotModel()
        {
            var model = new PlotModel
            {
                Title = GraphTitle,
                TitleFontSize = 14,
                TitleFontWeight = FontWeights.Bold,
                PlotAreaBorderColor = OxyColors.LightGray,
                PlotAreaBorderThickness = new OxyThickness(1)
            };

            // Ось X - время
            var timeAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = XAxisTitle,
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray
            };

            // Ось Y - количество
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = YAxisTitle,
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                Minimum = 0
            };

            model.Axes.Add(timeAxis);
            model.Axes.Add(valueAxis);

            // Серия для промышленных роботов
            var robotSeries = new LineSeries
            {
                Title = "Промышленные роботы",
                Color = OxyColors.DarkBlue,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.DarkBlue
            };

            // Серия для роботизированных рук
            var armSeries = new LineSeries
            {
                Title = "Роботизированные руки",
                Color = OxyColors.SteelBlue,
                StrokeThickness = 2,
                MarkerType = MarkerType.Triangle,
                MarkerSize = 4,
                MarkerFill = OxyColors.SteelBlue
            };

            // Серия для контроллеров
            var controllerSeries = new LineSeries
            {
                Title = "Контроллеры",
                Color = OxyColors.LightBlue,
                StrokeThickness = 2,
                MarkerType = MarkerType.Square,
                MarkerSize = 4,
                MarkerFill = OxyColors.LightBlue
            };

            // Серия для систем автоматизации
            var automationSeries = new LineSeries
            {
                Title = "Системы автоматизации",
                Color = OxyColors.Cyan,
                StrokeThickness = 2,
                MarkerType = MarkerType.Diamond,
                MarkerSize = 4,
                MarkerFill = OxyColors.Cyan
            };

            // Серия для использованных материалов
            var materialsSeries = new LineSeries
            {
                Title = "Использованные материалы",
                Color = OxyColors.Gray,
                StrokeThickness = 1.5,
                LineStyle = LineStyle.Dash,
                MarkerType = MarkerType.None
            };

            model.Series.Add(robotSeries);
            model.Series.Add(armSeries);
            model.Series.Add(controllerSeries);
            model.Series.Add(automationSeries);
            model.Series.Add(materialsSeries);

            // Обновляем данные
            UpdatePlotData(model);

            return model;
        }

        private void UpdatePlotData(PlotModel model)
        {
            var statistics = _productionService.GetStatistics();
            if (statistics == null || statistics.RoboticsHistory.Count == 0)
                return;

            var robotSeries = (LineSeries)model.Series[0];
            var armSeries = (LineSeries)model.Series[1];
            var controllerSeries = (LineSeries)model.Series[2];
            var automationSeries = (LineSeries)model.Series[3];
            var materialsSeries = (LineSeries)model.Series[4];

            robotSeries.Points.Clear();
            armSeries.Points.Clear();
            controllerSeries.Points.Clear();
            automationSeries.Points.Clear();
            materialsSeries.Points.Clear();

            for (int i = 0; i < statistics.RoboticsHistory.Count; i++)
            {
                var dataPoint = statistics.RoboticsHistory[i];

                robotSeries.Points.Add(new DataPoint(i, dataPoint.RoboticsProduction));
                armSeries.Points.Add(new DataPoint(i, dataPoint.RoboticsProduction / 2));
                controllerSeries.Points.Add(new DataPoint(i, dataPoint.RoboticsProduction * 2));
                automationSeries.Points.Add(new DataPoint(i, dataPoint.RoboticsProduction / 3));
                materialsSeries.Points.Add(new DataPoint(i, dataPoint.RoboticsMaterialsUsed));
            }
        }
    }
}