using Domain.Enums;

namespace Services.CommercialVisits
{
    public class CommercialVisitService : ICommercialVisitService
    {
        private readonly Dictionary<CommercialType, int> _totalVisits = new();
        private readonly CommercialVisitStatistics _statistics = new();

        public event Action StatisticsUpdated;

        public CommercialVisitService()
        {
            foreach (var type in _statistics.History.Keys)
            {
                _totalVisits[type] = 0;
            }
        }

        public void RecordVisit(CommercialType type, int tick)
        {
            if (!_statistics.History.ContainsKey(type))
                return;

            _totalVisits[type]++;
            _statistics.History[type].Add(
                new CommercialVisitDataPoint(tick, type, _totalVisits[type])
            );

            StatisticsUpdated?.Invoke();
        }

        public CommercialVisitStatistics GetStatistics() => _statistics;
    }
}

