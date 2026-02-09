using Domain.Enums;
using OxyPlot;
using OxyPlot.Series;
using Services.CommercialVisits;

namespace Services.Graphing
{
    public class CommercialVisitsGraphProvider : IGraphDataProvider
    {
        private readonly ICommercialVisitService _visitService;

        public CommercialVisitsGraphProvider(ICommercialVisitService visitService)
        {
            _visitService = visitService;
        }

        public string SystemName => "Посещения коммерции";
        public string GraphTitle => "Посещения коммерческих зданий";
        public string XAxisTitle => "Время";
        public string YAxisTitle => "Количество посещений";

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

            var stats = _visitService.GetStatistics().History;

            var lines = new Dictionary<CommercialType, LineSeries>
            {
                [CommercialType.Shop] = new LineSeries { Title = "Магазин", Color = OxyColors.SkyBlue },
                [CommercialType.Supermarket] = new LineSeries { Title = "Супермаркет", Color = OxyColors.Blue },
                [CommercialType.Cafe] = new LineSeries { Title = "Кафе", Color = OxyColors.Orange },
                [CommercialType.Pharmacy] = new LineSeries { Title = "Аптека", Color = OxyColors.Green },
                [CommercialType.Restaurant] = new LineSeries { Title = "Ресторан", Color = OxyColors.Purple },
                [CommercialType.GasStation] = new LineSeries { Title = "Заправка", Color = OxyColors.Red },
            };

            foreach (var kv in lines)
            {
                if (!stats.TryGetValue(kv.Key, out var points))
                    continue;

                foreach (var p in points)
                    kv.Value.Points.Add(new DataPoint(p.Tick, p.VisitCount));

                model.Series.Add(kv.Value);
            }

            return model;
        }
    }
}

