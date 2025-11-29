using Domain.Time;

namespace Services.Interfaces
{
    public interface IUpdatable
    {
        void Update(SimulationTime time);
    }
}
