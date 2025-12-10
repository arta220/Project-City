using Domain.Base;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Enums;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Citizens;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.EntityMovement.Service;
using Services.Time;

namespace Services.Citizens.Scenarios
{
    /// <summary>
    /// Сценарий выходного похода в парк.
    /// </summary>
    public class ParkVisitScenario : ICitizenScenario
    {
        private readonly IBuildingRegistry _registry;
        private readonly IEntityMovementService _movement;
        private readonly ISimulationTimeService _timeService;
        private readonly IParkVisitStatisticsService _statisticsService;
        private readonly Random _random = new();

        private const double VisitChance = 0.6;

        public ParkVisitScenario(
            IBuildingRegistry registry,
            IEntityMovementService movement,
            ISimulationTimeService timeService,
            IParkVisitStatisticsService statisticsService)
        {
            _registry = registry;
            _movement = movement;
            _timeService = timeService;
            _statisticsService = statisticsService;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            if (!time.IsWeekend())
                return false;

            return _registry.GetBuildings<Park>().Any();
        }

        public void BuildTasks(Citizen citizen)
        {
            var parks = _registry.GetBuildings<Park>().ToList();
            if (parks.Count == 0)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            if (_random.NextDouble() > VisitChance)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            var selectedPark = parks[_random.Next(parks.Count)];
            var (placement, found) = _registry.TryGetPlacement(selectedPark);

            if (!found || placement == null)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            citizen.State = CitizenState.GoingToPark;

            var firstTarget = PickRandomPosition((Placement)placement);
            citizen.Tasks.Enqueue(new MoveToPositionTask(firstTarget, _movement));
            citizen.Tasks.Enqueue(new WalkInParkTask(selectedPark, (Placement)placement, _movement, stepsToWalk: 5));

            _statisticsService.RecordVisit(selectedPark.Type, _timeService.CurrentTime.TotalTicks);
        }

        private Position PickRandomPosition(Placement placement)
        {
            var positions = placement.GetAllPositions().ToList();
            return positions[_random.Next(positions.Count)];
        }
    }
}

