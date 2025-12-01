using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Domain.Transports.Ground;
using Domain.Transports.States;
using Services.BuildingRegistry;
using Services.Citizens.Education;
using Services.Citizens.Movement;
using Services.Interfaces;
using Services.Time;

namespace Services.CitizensSimulation
{
    public class CitizenController
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly ICitizenMovementService _movement;
        private readonly IJobService _jobService;
        private readonly IEducationService _educationService;
        private readonly ISimulationTimeService _timeService;

        public CitizenController(
            IBuildingRegistry buildingRegistry,
            ICitizenMovementService movementService,
            IJobService jobService,
            IEducationService educationService,
            ISimulationTimeService timeService)
        {
            _buildingRegistry = buildingRegistry;
            _movement = movementService;
            _jobService = jobService;
            _educationService = educationService;
            _timeService = timeService;
        }

        public void UpdateCitizen(Citizen citizen, SimulationTime time)
        {
            switch (citizen.State)
            {
                case CitizenState.Idle:
                    DecideNextAction(citizen);
                    break;

                case CitizenState.GoingToWork:
                    UpdateGoingTo(citizen, time, citizen.WorkPlace);
                    break;

                case CitizenState.Working:
                    _jobService.UpdateWork(citizen, time);
                    break;

                case CitizenState.GoingToSchool:
                    UpdateGoingTo(citizen, time, citizen.StudyPlace);
                    break;

                case CitizenState.Studying:
                    _educationService.UpdateEducation(citizen, time);
                    break;

                case CitizenState.GoingHome:
                    UpdateGoingTo(citizen, time, citizen.Home);
                    break;

                case CitizenState.GoingToTransport:
                    UpdateGoingTo(citizen, time, citizen.Car!);
                    break;

                case CitizenState.InTransport:
                    UpdateInTransport(citizen, time);
                    break;
            }
        }

        private void DecideNextAction(Citizen citizen)
        {
            var timeOfDay = _timeService.GetTimeOfDay();
            var isWeekend = _timeService.IsWeekend();
            var isAtHome = IsAt(citizen, citizen.Home);

            if (!isAtHome && (timeOfDay == TimeOfDay.Evening || _timeService.IsNightTime()))
            {
                citizen.State = CitizenState.GoingHome;
                return;
            }

            if (timeOfDay == TimeOfDay.Morning && !isWeekend && isAtHome)
            {
                if (NeedsEducation(citizen) && !IsAt(citizen, citizen.StudyPlace))
                {
                    citizen.State = CitizenState.GoingToSchool;
                    return;
                }

                if (HasJob(citizen) && !IsAt(citizen, citizen.WorkPlace))
                {
                    citizen.State = citizen.HasCar ? CitizenState.GoingToTransport : CitizenState.GoingToWork;
                    return;
                }
            }

            if (citizen.State == CitizenState.GoingToWork && IsAt(citizen, citizen.WorkPlace))
            {
                citizen.State = CitizenState.Working;
                return;
            }

            if (citizen.State == CitizenState.GoingToSchool && IsAt(citizen, citizen.StudyPlace))
            {
                citizen.State = CitizenState.Studying;
                return;
            }

            if (isAtHome)
            {
                citizen.State = CitizenState.Idle;
            }
        }

        private bool IsAt(Citizen citizen, MapObject obj)
        {
            var (placement, found) = _buildingRegistry.TryGetPlacement(obj);
            return found && placement != null && citizen.Position.Equals(placement.Value.Position);
        }

        private bool HasJob(Citizen citizen) => citizen.WorkPlace != null;
        private bool NeedsEducation(Citizen citizen) => citizen.StudyPlace != null;

        private void UpdateGoingTo(Citizen citizen, SimulationTime time, MapObject to)
        {
            var (placement, found) = _buildingRegistry.TryGetPlacement(to);

            if (!found || placement == null)
            {
                citizen.State = CitizenState.Idle;
                citizen.CurrentPath.Clear();
                return;
            }

            var pos = placement.Value.Position;
            _movement.Move(citizen, pos, time);

            if (to is PersonalCar car && citizen.Position.Equals(car.Position))
            {
                citizen.State = CitizenState.InTransport;
            }
        }

        private void UpdateInTransport(Citizen citizen, SimulationTime time)
        {
            if (citizen.Car == null)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            citizen.Position = citizen.Car.Position;

            if (citizen.Car.State == TransportState.ParkedAtWork)
            {
                citizen.State = CitizenState.GoingToWork;
            }
            else if (citizen.Car.State == TransportState.IdleAtHome)
            {
                citizen.State = CitizenState.Idle;
            }
        }


    }
}
