using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using System.Linq;

namespace Services.Citizens.Job
{
    public class JobController
    {
        private readonly Dictionary<CitizenProfession, IJobBehaviour> _jobBehaviours;

        public JobController(IEnumerable<IJobBehaviour> behaviours)
        {
            _jobBehaviours = behaviours.ToDictionary(b => b.CitizenProfession);
        }

        public void UpdateJob(Citizen citizen, SimulationTime time)
        {
            if (_jobBehaviours.TryGetValue(citizen.Profession, out var behaviour))
            {
                behaviour.Update(citizen, time);
            }
        }
    }

}
