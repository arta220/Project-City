using Domain.Enums;

namespace Services.CommercialVisits
{
    public class CommercialVisitDataPoint
    {
        public int Tick { get; }
        public CommercialType CommercialType { get; }
        public int VisitCount { get; }

        public CommercialVisitDataPoint(int tick, CommercialType type, int visitCount)
        {
            Tick = tick;
            CommercialType = type;
            VisitCount = visitCount;
        }
    }
}

