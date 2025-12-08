using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Citizens.Movement;
using Services.CitizenSimulation.Tasks;
using Services.Time;

namespace Services.Citizens.Scenaries
{
    public class HomeScenario : ICitizenScenario
    {
        private readonly IBuildingRegistry _registry;
        private readonly ICitizenMovementService _movement;

        public HomeScenario(
            IBuildingRegistry registry,
            ICitizenMovementService movement)
        {
            _registry = registry;
            _movement = movement;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Ночь или вечер
            if (!(time.IsNightTime() || time.GetTimeOfDay() == TimeOfDay.Evening))
                return false;

            // Нет дома — нечего делать
            if (citizen.Home == null)
                return false;

            // Уже дома — не надо
            return !IsAtHome(citizen);
        }

        public void BuildTasks(Citizen citizen)
        {
            citizen.State = CitizenState.GoingHome;
            var entrance = GetEntrance(citizen.Home);


            citizen.Tasks.Enqueue(
                new MoveToPositionTask(entrance, _movement)
            );
        }

        private bool IsAtHome(Citizen citizen)
        {
            var (pl, ok) = _registry.TryGetPlacement(citizen.Home);
            return ok && citizen.Position == pl.Value.Entrance;
        }

        private Position GetEntrance(MapObject building)
        {
            var (pl, _) = _registry.TryGetPlacement(building);
            return pl.Value.Entrance;
        }
    }
}