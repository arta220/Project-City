using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;

namespace Services.Interfaces
{
    public interface IJobService
    {
        CitizenJob? FindJobFor(Citizen citizen);
        void UpdateWork(Citizen citizen, SimulationTime time); 
    }
}
