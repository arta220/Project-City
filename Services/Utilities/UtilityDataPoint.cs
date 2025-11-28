namespace Services.Utilities
{
    public class UtilityDataPoint
    {
        public int Tick { get; set; }
        public int BreakdownCount { get; set; }
        public int RepairCount { get; set; }

        public UtilityDataPoint(int tick, int breakdownCount, int repairCount)
        {
            Tick = tick;
            BreakdownCount = breakdownCount;
            RepairCount = repairCount;
        }
    }
}
