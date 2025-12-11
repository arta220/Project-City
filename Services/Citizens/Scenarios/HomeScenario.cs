using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Citizens.Tasks;
using Services.EntityMovement.Service;
using Services.Time;

namespace Services.Citizens.Scenaries
{
    public class HomeScenario : ICitizenScenario
    {
        private readonly IBuildingRegistry _registry;
        private readonly IEntityMovementService _movement;

        public HomeScenario(
            IBuildingRegistry registry,
            IEntityMovementService movement)
        {
            _registry = registry;
            _movement = movement;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            if (!(time.IsNightTime() || time.GetTimeOfDay() == TimeOfDay.Evening))
                return false;

            if (citizen.Home == null)
                return false;

            return !IsAtHome(citizen);
        }

        public void BuildTasks(Citizen citizen)
        {
            citizen.State = CitizenState.GoingHome;

            citizen.Tasks.Enqueue(
                new MoveToBuildingTask(citizen.Home!, _movement, _registry)
            );
        }

        private bool IsAtHome(Citizen citizen)
        {
            var (pl, ok) = _registry.TryGetPlacement(citizen.Home!);
            return ok && citizen.Position == pl.Value.Entrance;
        }
    }
}
