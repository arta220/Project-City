using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.EntityMovement.Service;

namespace Services.Citizens.Tasks.Move;
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

            var accessibleTiles = placement!.Value // "Вход" в здание считается доступным, если вокруг здания есть хотя бы одна пустая клетка.
                .GetAllPositions()
                .SelectMany(pos => pos.GetNeighbors())
                .Where(tilePos => citizen.NavigationProfile.CanEnter(tilePos));


            if (!accessibleTiles.Any())
            {
                citizen.State = CitizenState.Idle;
                IsCompleted = true;
                return;
            }

            _movement.SetTarget(citizen, accessibleTiles.First());
            _pathSet = true;
        }

        _movement.PlayMovement(citizen, time);

        if (citizen.Position == citizen.TargetPosition)
            IsCompleted = true;
    }
}
