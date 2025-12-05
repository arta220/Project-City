using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
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
            // 1. Ищем работу через сервис
            var availableJobs = _jobService.FindJob(citizen.Profession)
                                      .Cast<Building>()
                                      .Where(job => IsJobSuitable(citizen, job))
                                      .ToList();

            if (availableJobs.Any())
            {
                // 2. Берем первую найденную работу
                var job = availableJobs.First() as Building;
                // 3. Пытаемся наняться
                if (job.Hire(citizen))
                {
                    return;
                }
            }

            // 4. Если работу не нашли или не смогли наняться
            citizen.State = CitizenState.Idle;
        }

        private bool IsJobSuitable(Citizen citizen, Building job)
        {
            // Есть ли вакансия
            if (!job.HasVacancy(citizen.Profession))
                return false;

            // Подходит ли по возрасту
            // Если MaxAges не содержит профессию - ограничений нет
            if (job.MaxAges.ContainsKey(citizen.Profession))
            {
                return citizen.Age <= job.MaxAges[citizen.Profession];
            }

            return true;
        }
    }
}
