using OxyPlot;
using OxyPlot.Series;
using Services.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Graphing
{
    /// <summary>
    /// Поставщик данных для графика изменения природных ресурсов на карте.
    /// </summary>
    public class ResourcesGraphProvider : IGraphDataProvider
    {
        private readonly IResourceStatisticsService _resourceStatsService;

        public string SystemName => "Природные ресурсы";
        public string GraphTitle => "Динамика ресурсов на карте";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Суммарное количество ресурса";

        public ResourcesGraphProvider(IResourceStatisticsService resourceStatsService)
        {
            _resourceStatsService = resourceStatsService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _resourceStatsService.GetStatistics();

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

            // Линии по типам ресурса
            var ironLine = new LineSeries { Title = "Железо" };
            var copperLine = new LineSeries { Title = "Медь" };
            var oilLine = new LineSeries { Title = "Нефть" };
            var gasLine = new LineSeries { Title = "Газ" };
            var woodLine = new LineSeries { Title = "Дерево" };

            foreach (var snap in statistics.History)
            {
                ironLine.Points.Add(new DataPoint(snap.Tick, snap.Iron));
                copperLine.Points.Add(new DataPoint(snap.Tick, snap.Copper));
                oilLine.Points.Add(new DataPoint(snap.Tick, snap.Oil));
                gasLine.Points.Add(new DataPoint(snap.Tick, snap.Gas));
                woodLine.Points.Add(new DataPoint(snap.Tick, snap.Wood));
            }

            plotModel.Series.Add(ironLine);
            plotModel.Series.Add(copperLine);
            plotModel.Series.Add(oilLine);
            plotModel.Series.Add(gasLine);
            plotModel.Series.Add(woodLine);

            return plotModel;
        }
    }
}
