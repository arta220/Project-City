// DisasterDataPoint.cs
namespace Services.Disasters
{
    public class DisasterDataPoint
    {
        public int Tick { get; set; }
        public int ActiveCount { get; set; } // Количество АКТИВНЫХ бедствий на этот тик

        public DisasterDataPoint(int tick, int activeCount)
        {
            Tick = tick;
            ActiveCount = activeCount;
        }
    }
}