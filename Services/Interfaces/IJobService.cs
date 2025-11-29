using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Time;

namespace Services.Interfaces
{
    public interface IJobService
    {
        Job? FindJobFor(Citizen citizen);
        void UpdateWork(Citizen citizen, SimulationTime time); 
    }
}
