using Services.Interfaces;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;

namespace Services.Citizens.Job
{
    public class JobService : IJobService
    {
        public CitizenJob? FindJobFor(Citizen citizen)
        {
            throw new NotImplementedException();
        }

        public void UpdateWork(Citizen citizen, SimulationTime time)
        {
 //           throw new NotImplementedException();
        }
    }
}
