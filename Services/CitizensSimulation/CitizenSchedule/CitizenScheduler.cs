using Domain.Citizens;
using Domain.Citizens.States;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Time;

namespace Services.CitizensSimulation.CitizenSchedule
{
    public class CitizenScheduler : ICitizenScheduler
    {
        private readonly ISimulationTimeService _time;
        private readonly IBuildingRegistry _registry;
        private readonly IEnumerable<ICitizenScenario> _scenarios;

        public CitizenScheduler(
            ISimulationTimeService time,
            IBuildingRegistry registry,
            IEnumerable<ICitizenScenario> scenarios)
        {
            _time = time;
            _registry = registry;
            _scenarios = scenarios;
        }

        public void UpdateSchedule(Citizen citizen)
        {
            // Если уже есть задачи — не пересчитываем расписание
            if (citizen.CurrentTask != null || citizen.Tasks.Count > 0)
                return;

            foreach (var scenario in _scenarios)
            {
                if (scenario.CanRun(citizen, _time))
                {
                    scenario.BuildTasks(citizen);
                    return;
                }
            }

            citizen.State = CitizenState.Idle;
        }
    }
}