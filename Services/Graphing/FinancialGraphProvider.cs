using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Services.Finance;
using System.Collections.Generic;
using System.Linq;
using Domain.Finance;

namespace Services.Graphing
{
    /// <summary>
    /// Предоставление данных для построения графиков финансовой статистики
    /// </summary>
    public class FinancialGraphProvider : IGraphDataProvider
    {
        private readonly IFinanceService _financeService;

        /// <summary>
        /// Получает названия системы, к которой относятся данные графика
        /// </summary>
        public string SystemName => "Финансы";

        /// <summary>
        /// Получает заголовок графика
        /// </summary>
        public string GraphTitle => "Финансовая статистика";

        /// <summary>
        /// Получает заголовок оси X графика
        /// </summary>
        public string XAxisTitle => "Время (тики)";

        /// <summary>
        /// Получает заголовок оси Y графика
        /// </summary>
        public string YAxisTitle => "Сумма";

        /// <summary>
        /// Инициализация нового экземпляра класса <see cref="FinancialGraphProvider"/>
        /// </summary>
        /// <param name="financeService">Сервис управления финансами</param>
        public FinancialGraphProvider(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        /// <summary>
        /// Создание модели графика финансовой статистики
        /// </summary>
        /// <returns>Модель графика с линией баланса</returns>
        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = GraphTitle };

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = XAxisTitle
            });
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = YAxisTitle
            });

            var balanceSeries = new LineSeries { Title = "Баланс", Color = OxyColors.Green, StrokeThickness = 3 };
            plotModel.Series.Add(balanceSeries);

            var history = _financeService.Statistics.History;

            // Если истории нет, добавление точки с начальным балансом для корректного отображения
            if (!history.Any())
            {
                balanceSeries.Points.Add(new DataPoint(0, 8000000));
                return plotModel;
            }

            // Рисует только линию баланса (единый главный график)
            foreach (var dataPoint in history)
            {
                balanceSeries.Points.Add(new DataPoint(dataPoint.Tick, dataPoint.Balance));
            }

            return plotModel;
        }
    }
}