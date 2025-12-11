using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.EntityMovement.Service;

namespace Services.Citizens.Tasks
{
    public class MoveToBuildingTask : ICitizenTask
    {
        private readonly MapObject _targetBuilding;
        private readonly IEntityMovementService _movement;
        private readonly IBuildingRegistry _buildingRegistry;

        private bool _pathSet = false;
        public bool IsCompleted { get; private set; }

        public MoveToBuildingTask(
            MapObject targetBuilding,
            IEntityMovementService movement,
            IBuildingRegistry buildingRegistry)
        {
            _targetBuilding = targetBuilding;
            _movement = movement;
            _buildingRegistry = buildingRegistry;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            if (!_pathSet)
            {
                var (placement, found) = _buildingRegistry.TryGetPlacement(_targetBuilding);
                if (!found)
                {
                    citizen.State = CitizenState.Idle;
                    IsCompleted = true;
                    return;
                }

                var entrance = placement!.Value.Entrance;
                var neighbors = placement.Value
                    .GetAllPositions()
                    .SelectMany(pos => pos.GetNeighbors());

                // Предпочитаем вход, если он проходим
                if (citizen.NavigationProfile.CanEnter(entrance))
                {
                    _movement.SetTarget(citizen, entrance);
                }
                else
                {
                    var accessibleTiles = neighbors
                        .Where(tilePos => citizen.NavigationProfile.CanEnter(tilePos));

                    if (!accessibleTiles.Any())
                    {
                        citizen.State = CitizenState.Idle;
                        IsCompleted = true;
                        return;
                    }

                    _movement.SetTarget(citizen, accessibleTiles.First());
                }
                _pathSet = true;
            }

            _movement.PlayMovement(citizen, time);

            if (citizen.Position == citizen.TargetPosition)
                IsCompleted = true;
        }
    }
}
