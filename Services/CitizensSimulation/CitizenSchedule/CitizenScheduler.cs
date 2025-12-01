using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Time;

namespace Services.CitizensSimulation.CitizenSchedule
{
    public class CitizenScheduler : ICitizenScheduler
    {
        private readonly ISimulationTimeService _timeService;
        private readonly IBuildingRegistry _buildingRegistry;

        public CitizenScheduler(
            ISimulationTimeService timeService,
            IBuildingRegistry buildingRegistry)
        {
            _timeService = timeService;
            _buildingRegistry = buildingRegistry;
        }

        public CitizenState Decide(Citizen citizen, CitizenState current, SimulationTime time)
            => DecideNextAction(citizen, current);

        private CitizenState DecideNextAction(Citizen citizen, CitizenState current)
        {
            var timeOfDay = _timeService.GetTimeOfDay();
            var isWeekend = _timeService.IsWeekend();
            var isAtHome = IsAt(citizen, citizen.Home);

            if (!isAtHome && (timeOfDay == TimeOfDay.Evening || _timeService.IsNightTime()))
                return CitizenState.GoingHome;

            if (timeOfDay == TimeOfDay.Morning && !isWeekend && isAtHome)
            {
                if (NeedsEducation(citizen) && !IsAt(citizen, citizen.StudyPlace))
                    return CitizenState.GoingToStudy;

                if (HasJob(citizen) && !IsAt(citizen, citizen.WorkPlace))
                    return citizen.HasCar ? CitizenState.GoingToTransport : CitizenState.GoingToWork;
            }

            if (current == CitizenState.GoingToWork && IsAt(citizen, citizen.WorkPlace))
                return CitizenState.Working;

            if (current == CitizenState.GoingToStudy && IsAt(citizen, citizen.StudyPlace))
                return CitizenState.Studying;

            return CitizenState.Idle;
        }

        private bool IsAt(Citizen citizen, MapObject obj)
        {
            var (placement, found) = _buildingRegistry.TryGetPlacement(obj);
            return found &&
                   placement != null &&
                   citizen.Position.Equals(placement.Value.Position);
        }

        private bool HasJob(Citizen citizen) => citizen.WorkPlace != null;
        private bool NeedsEducation(Citizen citizen) => citizen.StudyPlace != null;
    }
}
