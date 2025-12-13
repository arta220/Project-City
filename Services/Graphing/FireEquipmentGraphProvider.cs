using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Services.IndustrialProduction;

namespace Services.Graphing
{
    /// <summary>
    /// Провайдер графиков для производства противопожарного оборудования
    /// </summary>
    public class FireEquipmentGraphProvider : IGraphDataProvider
    {
        private readonly IIndustrialProductionService _productionService;

        public string SystemName => "Противопожарное оборудование";
        public string GraphTitle => "Производство противопожарного оборудования";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public FireEquipmentGraphProvider(IIndustrialProductionService productionService)
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

            // Ось X
            var timeAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = XAxisTitle,
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray
            };

            // Ось Y
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

            // Серия для огнетушителей
            var extinguisherSeries = new LineSeries
            {
                Title = "Огнетушители",
                Color = OxyColors.Red,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Red
            };

            // Серия для пожарных рукавов
            var hoseSeries = new LineSeries
            {
                Title = "Пожарные рукава",
                Color = OxyColors.Orange,
                StrokeThickness = 2,
                MarkerType = MarkerType.Triangle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Orange
            };

            // Серия для систем сигнализации
            var alarmSeries = new LineSeries
            {
                Title = "Системы сигнализации",
                Color = OxyColors.Yellow,
                StrokeThickness = 2,
                MarkerType = MarkerType.Square,
                MarkerSize = 4,
                MarkerFill = OxyColors.Yellow
            };

            // Серия для пожарных машин
            var truckSeries = new LineSeries
            {
                Title = "Пожарные машины",
                Color = OxyColors.DarkRed,
                StrokeThickness = 2,
                MarkerType = MarkerType.Diamond,
                MarkerSize = 4,
                MarkerFill = OxyColors.DarkRed
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

            model.Series.Add(extinguisherSeries);
            model.Series.Add(hoseSeries);
            model.Series.Add(alarmSeries);
            model.Series.Add(truckSeries);
            model.Series.Add(materialsSeries);

            // Обновляем данные
            UpdatePlotData(model);

            return model;
        }

        private void UpdatePlotData(PlotModel model)
        {
            var statistics = _productionService.GetStatistics();
            if (statistics == null || statistics.FireEquipmentHistory.Count == 0)
                return;

            var extinguisherSeries = (LineSeries)model.Series[0];
            var hoseSeries = (LineSeries)model.Series[1];
            var alarmSeries = (LineSeries)model.Series[2];
            var truckSeries = (LineSeries)model.Series[3];
            var materialsSeries = (LineSeries)model.Series[4];

            extinguisherSeries.Points.Clear();
            hoseSeries.Points.Clear();
            alarmSeries.Points.Clear();
            truckSeries.Points.Clear();
            materialsSeries.Points.Clear();

            for (int i = 0; i < statistics.FireEquipmentHistory.Count; i++)
            {
                var dataPoint = statistics.FireEquipmentHistory[i];

                extinguisherSeries.Points.Add(new DataPoint(i, dataPoint.FireEquipmentProduction));
                hoseSeries.Points.Add(new DataPoint(i, dataPoint.FireEquipmentProduction * 2));
                alarmSeries.Points.Add(new DataPoint(i, dataPoint.FireEquipmentProduction / 2));
                truckSeries.Points.Add(new DataPoint(i, dataPoint.FireEquipmentProduction / 10));
                materialsSeries.Points.Add(new DataPoint(i, dataPoint.FireEquipmentMaterialsUsed));
            }
        }
    }
}