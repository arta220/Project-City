using Services.Common;

namespace Services.JewelryProduction
{
    public interface IJewelryProductionService : IUpdatable
    {
        JewelryProductionStatistics GetStatistics();
    }
}
