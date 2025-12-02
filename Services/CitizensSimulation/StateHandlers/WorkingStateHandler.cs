using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Services.Citizens.Job;

namespace Services.CitizensSimulation.StateHandlers
{
    public class WorkingStateHandler : ICitizenStateHandler
    {
        private readonly JobController _jobController;

        public WorkingStateHandler(JobController jobController)
        {
            _jobController = jobController;
        }
        public bool CanHandle(CitizenState state) => state == CitizenState.Working;

        public void Update(Citizen citizen, SimulationTime time) => _jobController.UpdateJob(citizen, time);
    }
}
