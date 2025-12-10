using Domain.Common.Enums;
using OxyPlot;
using OxyPlot.Series;
using Services.Citizens;

namespace Services.Graphing
{
    public class ParkVisitsGraphProvider : IGraphDataProvider
    {
        private readonly IParkVisitStatisticsService _statisticsService;

        public string SystemName => "Парки";
        public string GraphTitle => "Посещения парков";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество посещений";

        public ParkVisitsGraphProvider(IParkVisitStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public PlotModel CreatePlotModel()
        {
            var model = new PlotModel { Title = GraphTitle };
            model.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle
            });
            model.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = YAxisTitle
            });

            var statistics = _statisticsService.GetStatistics();

            foreach (ParkType parkType in Enum.GetValues(typeof(ParkType)))
            {
                var series = new LineSeries
                {
                    Title = parkType.ToString(),
                    Color = GetColorForPark(parkType)
                };

                foreach (var data in statistics.History[parkType])
                {
                    series.Points.Add(new DataPoint(data.Tick, data.Visits));
                }

                model.Series.Add(series);
            }

            return model;
        }

        private static OxyColor GetColorForPark(ParkType type) =>
            type switch
            {
                ParkType.UrbanPark => OxyColors.ForestGreen,
                ParkType.BotanicalGarden => OxyColors.SeaGreen,
                ParkType.Playground => OxyColors.Goldenrod,
                ParkType.Square => OxyColors.LightSeaGreen,
                ParkType.RecreationArea => OxyColors.MediumPurple,
                _ => OxyColors.Gray
            };
    }
}

