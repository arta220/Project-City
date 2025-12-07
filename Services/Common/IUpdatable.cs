using Domain.Common.Time;

namespace Services.Common
{
    public interface IUpdatable
    {
        void Update(SimulationTime time);
    }
}
