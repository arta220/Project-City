using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Services.Interfaces;

namespace Services.CitizensSimulation.StateHandlers
{
    public class SearchingWorkStateHandler : ICitizenStateHandler
    {
        private readonly IFindJobService _jobService;

        public SearchingWorkStateHandler(IFindJobService jobService)
        {
            _jobService = jobService;
        }
        public bool CanHandle(CitizenState state) => state == CitizenState.SearchingWork;

        public void Update(Citizen citizen, SimulationTime time)
        {
            var v = _jobService.FindJob(citizen.Profession);

            if (v.Count() > 0)
            {
                citizen.WorkPlace = v.First();
                // можно добавить избирательность для жителя, в зависимости от расстояния, зарплат и тп
                //но эт не моё
            }
            citizen.State = CitizenState.Idle;
        }
    }
}
