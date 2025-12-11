using Domain.Enums;

namespace Services.CommercialVisits
{
    public interface ICommercialVisitService
    {
        void RecordVisit(CommercialType type, int tick);
        CommercialVisitStatistics GetStatistics();
        event Action StatisticsUpdated;
    }
}

