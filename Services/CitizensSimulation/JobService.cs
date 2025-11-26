using Services.Interfaces;
using Domain.Citizens;
using Domain.Citizens.States;

namespace Services.CitizensSimulation
{
    public class JobService : IJobService
    {
        public Job? FindJobFor(Citizen citizen)
        {
            throw new NotImplementedException();
        }

        public void UpdateWork(Citizen citizen, int tick)
        {
            throw new NotImplementedException();
        }
    }
}
