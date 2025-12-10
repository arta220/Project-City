using Domain.Common.Enums;

namespace Services.Citizens
{
    public record ParkVisitDataPoint(int Tick, int Visits);

    public class ParkVisitStatistics
    {
        public Dictionary<ParkType, List<ParkVisitDataPoint>> History { get; } = Enum
            .GetValues(typeof(ParkType))
            .Cast<ParkType>()
            .ToDictionary(t => t, _ => new List<ParkVisitDataPoint>());
    }

    public interface IParkVisitStatisticsService
    {
        void RecordVisit(ParkType parkType, int tick);
        ParkVisitStatistics GetStatistics();
    }

    public class ParkVisitStatisticsService : IParkVisitStatisticsService
    {
        private readonly ParkVisitStatistics _statistics = new();
        private readonly Dictionary<ParkType, int> _totals;

        public ParkVisitStatisticsService()
        {
            _totals = Enum.GetValues(typeof(ParkType))
                .Cast<ParkType>()
                .ToDictionary(t => t, _ => 0);
        }

        public void RecordVisit(ParkType parkType, int tick)
        {
            _totals[parkType]++;
            _statistics.History[parkType].Add(new ParkVisitDataPoint(tick, _totals[parkType]));
        }

        public ParkVisitStatistics GetStatistics() => _statistics;
    }
}

