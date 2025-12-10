using System.Collections.Generic;

namespace Services.Finance
{
    public class FinanceStatistics
    {
        /// <summary>
        /// Получает или задает список точек финансовых данных, представляющих историю финансового состояния
        /// </summary>
        /// <value>
        /// Коллекция объектов <see cref="FinanceDataPoint"/>, содержащая финансовые данные по каждому тику симуляции
        /// По умолчанию инициализируется пустым списком
        /// </value>
        public List<FinanceDataPoint> History { get; set; } = new List<FinanceDataPoint>();
    }
}