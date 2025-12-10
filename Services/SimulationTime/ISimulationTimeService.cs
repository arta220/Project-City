using Domain.Common.Time;

namespace Services.Time
{
    public interface ISimulationTimeService
    {
        SimulationTime CurrentTime { get; }
        event Action<SimulationTime> TimeChanged;
        event Action NewDay;
        event Action NewMonth;
        event Action NewYear;
        event Action<Season> SeasonChanged;

        bool IsWorkTime();
        bool IsNightTime();
        bool IsWeekend();
        TimeOfDay GetTimeOfDay();
        void SetCurrentTime(int tick);
    }
}
