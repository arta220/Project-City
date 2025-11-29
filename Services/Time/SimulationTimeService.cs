using Domain.Time;
using Services.Time.Clock;

namespace Services.Time
{
    public class SimulationTimeService : ISimulationTimeService
    {
        private readonly ISimulationClock _clock;
        private int _lastDay;
        private int _lastMonth;
        private int _lastYear;
        private Season _lastSeason;

        public SimulationTime CurrentTime { get; private set; }

        public event Action<SimulationTime> TimeChanged;
        public event Action NewDay;
        public event Action NewMonth;
        public event Action NewYear;
        public event Action<Season> SeasonChanged;

        public SimulationTimeService(ISimulationClock clock)
        {
            _clock = clock;
            _clock.TickOccurred += OnTick;

            _clock.Start();

            CurrentTime = new SimulationTime(_clock.CurrentTick);
            UpdateState();
        }

        private void OnTick(int tick)
        {
            var previousTime = CurrentTime;
            CurrentTime = new SimulationTime(tick);

            CheckDayChange(previousTime);
            CheckMonthChange(previousTime);
            CheckYearChange(previousTime);
            CheckSeasonChange(previousTime);

            TimeChanged?.Invoke(CurrentTime);
        }

        private void CheckDayChange(SimulationTime previousTime)
        {
            if (CurrentTime.Day != previousTime.Day)
            {
                NewDay?.Invoke();
            }
        }

        private void CheckMonthChange(SimulationTime previousTime)
        {
            if (CurrentTime.Month != previousTime.Month)
            {
                NewMonth?.Invoke();
            }
        }

        private void CheckYearChange(SimulationTime previousTime)
        {
            if (CurrentTime.Year != previousTime.Year)
            {
                NewYear?.Invoke();
            }
        }

        private void CheckSeasonChange(SimulationTime previousTime)
        {
            if (CurrentTime.Season != previousTime.Season)
            {
                SeasonChanged?.Invoke(CurrentTime.Season);
            }
        }

        private void UpdateState()
        {
            _lastDay = CurrentTime.Day;
            _lastMonth = CurrentTime.Month;
            _lastYear = CurrentTime.Year;
            _lastSeason = CurrentTime.Season;
        }

        public bool IsWorkTime() => CurrentTime.IsWorkTime;
        public bool IsNightTime() => CurrentTime.IsNight;
        public bool IsWeekend() => CurrentTime.IsWeekend;

        public TimeOfDay GetTimeOfDay()
        {
            return CurrentTime.Hour switch
            {
                >= 0 and < 6 => TimeOfDay.Night,
                >= 6 and < 9 => TimeOfDay.Morning,
                >= 9 and < 18 => TimeOfDay.Day,
                _ => TimeOfDay.Evening
            };
        }
    }
}