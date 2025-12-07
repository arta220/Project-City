using OxyPlot;
using OxyPlot.Series;
using Services.GlassProduction;

namespace Services.Graphing
{
    /// <summary>
    /// Провайдер графика для производства стекольных изделий
    /// </summary>
    public class GlassGraphProvider : IGraphDataProvider
    {
        private readonly IGlassProductionService _glassService;

        public string SystemName => "Стекольное производство";
        public string GraphTitle => "Статистика стекольного производства";
        public string XAxisTitle => "Время (тики)";
        public string YAxisTitle => "Количество";

        public GlassGraphProvider(IGlassProductionService glassService)
        {
            _glassService = glassService;
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };
            var statistics = _glassService.GetStatistics();

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

            // Линия общего производства
            var totalProductionLine = new LineSeries 
            { 
                Title = "Всего изделий", 
                Color = OxyColors.Black,
                StrokeThickness = 2
            };

            // Категории изделий
            var bottlesLine = new LineSeries 
            { 
                Title = "Бутылки", 
                Color = OxyColors.LightBlue 
            };

            var vasesLine = new LineSeries 
            { 
                Title = "Вазы", 
                Color = OxyColors.Cyan 
            };

            var windowsLine = new LineSeries 
            { 
                Title = "Окна", 
                Color = OxyColors.SkyBlue 
            };

            var mirrorsLine = new LineSeries 
            { 
                Title = "Зеркала", 
                Color = OxyColors.Silver 
            };

            var tablewareLine = new LineSeries 
            { 
                Title = "Посуда", 
                Color = OxyColors.LightGreen 
            };

            // Премиум и эксклюзивные линии
            var premiumLine = new LineSeries 
            { 
                Title = "Премиум изделия", 
                Color = OxyColors.Orange,
                StrokeThickness = 2
            };

            var exclusiveLine = new LineSeries 
            { 
                Title = "Эксклюзивные изделия", 
                Color = OxyColors.Magenta,
                StrokeThickness = 2
            };

            // Линия использованных материалов
            var materialsLine = new LineSeries 
            { 
                Title = "Материалы", 
                Color = OxyColors.Gray,
                LineStyle = LineStyle.Dash
            };

            // Заполнение данными
            foreach (var dataPoint in statistics.ProductionHistory)
            {
                totalProductionLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.TotalProduction));
                bottlesLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.BottlesProduction));
                vasesLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.VasesProduction));
                windowsLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.WindowsProduction));
                mirrorsLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.MirrorsProduction));
                tablewareLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.TablewareProduction));
                premiumLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.PremiumProduction));
                exclusiveLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.ExclusiveProduction));
                materialsLine.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.MaterialsUsed));
            }

            // Добавление всех линий в график
            plotModel.Series.Add(totalProductionLine);
            plotModel.Series.Add(bottlesLine);
            plotModel.Series.Add(vasesLine);
            plotModel.Series.Add(windowsLine);
            plotModel.Series.Add(mirrorsLine);
            plotModel.Series.Add(tablewareLine);
            plotModel.Series.Add(premiumLine);
            plotModel.Series.Add(exclusiveLine);
            plotModel.Series.Add(materialsLine);

            return plotModel;
        }
    }
}
