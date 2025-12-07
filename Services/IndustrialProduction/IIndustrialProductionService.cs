using Services.Common;

namespace Services.IndustrialProduction
{
    public interface IIndustrialProductionService : IUpdatable
    {
        IndustrialProductionStatistics GetStatistics();
    }
}

