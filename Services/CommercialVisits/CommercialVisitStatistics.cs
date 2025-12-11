using Domain.Enums;

namespace Services.CommercialVisits
{
    public class CommercialVisitStatistics
    {
        public Dictionary<CommercialType, List<CommercialVisitDataPoint>> History { get; } = new();

        public CommercialVisitStatistics()
        {
            foreach (CommercialType type in Enum.GetValues(typeof(CommercialType)))
            {
                if (type == CommercialType.Service)
                    continue;

                // Начинаем с точки (tick 0, visits 0) для отображения линий
                History[type] = new List<CommercialVisitDataPoint>
                {
                    new CommercialVisitDataPoint(0, type, 0)
                };
            }
        }
    }
}

