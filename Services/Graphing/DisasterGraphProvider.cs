// DisasterGraphProvider.cs
using OxyPlot;
using OxyPlot.Series;
using Services.Disasters;
using System.Linq;

namespace Services.Graphing
{
    /// <summary>
    /// Провайдер графика для статистики стихийных бедствий.
    /// Показывает количество АКТИВНЫХ бедствий в каждый момент времени.
    /// </summary>
    public class DisasterGraphProvider : IGraphDataProvider
    {
        private readonly IDisasterService _disasterService;

        public string SystemName => "Стихийные бедствия";
        public string GraphTitle => "Активные стихийные бедствия по времени";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество активных бедствий";

        public DisasterGraphProvider(IDisasterService disasterService)
        {
            _disasterService = disasterService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _disasterService.GetStatistics();

            // Добавление осей
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = XAxisTitle,
                Minimum = 0
            });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = YAxisTitle,
                Minimum = 0
            });

            // Линия для АКТИВНЫХ пожаров
            var fireLine = new LineSeries
            {
                Title = "Пожары",
                Color = OxyColors.Red,
                StrokeThickness = 2
            };

            // Линия для АКТИВНЫХ наводнений
            var floodLine = new LineSeries
            {
                Title = "Наводнения",
                Color = OxyColors.DarkBlue,
                StrokeThickness = 2
            };

            // Линия для АКТИВНЫХ метелей
            var blizzardLine = new LineSeries
            {
                Title = "Метели",
                Color = OxyColors.LightBlue,
                StrokeThickness = 2
            };

            // Заполняем данными - количество активных на каждый тик
            FillSeriesWithActiveCount(fireLine, statistics.FireHistory);
            FillSeriesWithActiveCount(floodLine, statistics.FloodHistory);
            FillSeriesWithActiveCount(blizzardLine, statistics.BlizzardHistory);

            // Если данных нет, добавляем начальную точку
            if (!statistics.FireHistory.Any())
            {
                fireLine.Points.Add(new DataPoint(0, 0));
            }
            if (!statistics.FloodHistory.Any())
            {
                floodLine.Points.Add(new DataPoint(0, 0));
            }
            if (!statistics.BlizzardHistory.Any())
            {
                blizzardLine.Points.Add(new DataPoint(0, 0));
            }

            plotModel.Series.Add(fireLine);
            plotModel.Series.Add(floodLine);
            plotModel.Series.Add(blizzardLine);

            return plotModel;
        }

        private void FillSeriesWithActiveCount(LineSeries series, List<DisasterDataPoint> history)
        {
            if (!history.Any()) return;

            System.Diagnostics.Debug.WriteLine($"[GraphProvider] Filling series with {history.Count} points");

            // Для активных бедствий важно сохранять все точки для точного отображения
            // когда бедствия начинаются и заканчиваются

            // Ограничиваем количество точек для производительности, но сохраняем важные изменения
            if (history.Count > 500)
            {
                // Прореживаем, но сохраняем точки где значение меняется
                int? lastValue = null;
                for (int i = 0; i < history.Count; i++)
                {
                    var point = history[i];

                    // Сохраняем точку если значение изменилось или это первая/последняя точка
                    if (lastValue == null || point.ActiveCount != lastValue ||
                        i == 0 || i == history.Count - 1 || i % 50 == 0)
                    {
                        series.Points.Add(new DataPoint(point.Tick, point.ActiveCount));
                        lastValue = point.ActiveCount;
                    }
                }
            }
            else
            {
                // Если точек немного, используем все
                foreach (var point in history)
                {
                    series.Points.Add(new DataPoint(point.Tick, point.ActiveCount));
                }
            }
        }
    }
}