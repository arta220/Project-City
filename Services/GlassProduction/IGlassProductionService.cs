using Services.Common;

namespace Services.GlassProduction
{
    public interface IGlassProductionService : IUpdatable
    {
        GlassProductionStatistics GetStatistics();
    }
}
