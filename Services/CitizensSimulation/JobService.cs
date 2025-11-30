using Services.Interfaces;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Time;

namespace Services.CitizensSimulation
{
    public class JobService : IJobService
    {
        public Job? FindJobFor(Citizen citizen)
        {
            throw new NotImplementedException();
        }

        public void UpdateWork(Citizen citizen, SimulationTime time)
        {
            throw new NotImplementedException();
        }
    }
}
