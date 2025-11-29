namespace Domain.Time
{
    public class SimulationTime
    {
        public int TotalTicks { get; }
        public int Day { get; }
        public int Month { get; }
        public int Year { get; }
        public int Hour { get; }
        public int Minute { get; }
        public DayOfWeek DayOfWeek { get; }
        public Season Season { get; }
        public bool IsWeekend => DayOfWeek == DayOfWeek.Saturday || DayOfWeek == DayOfWeek.Sunday;
        public bool IsNight => Hour >= 22 || Hour < 6;
        public bool IsWorkTime => Hour >= 9 && Hour < 18 && !IsWeekend;

        public SimulationTime(int totalTicks)
        {
            TotalTicks = totalTicks;

            var totalMinutes = totalTicks;
            var totalHours = totalMinutes / 60;
            var totalDays = totalHours / 24;

            Minute = totalMinutes % 60;
            Hour = totalHours % 24;
            Day = (totalDays % 30) + 1;
            Month = ((totalDays / 30) % 12) + 1;
            Year = (totalDays / (30 * 12)) + 1;

            DayOfWeek = (DayOfWeek)(totalDays % 7);
            Season = (Season)((Month - 1) / 3);
        }
    }
}
