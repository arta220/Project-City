using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Citizens.Movement;

namespace Services.CitizensSimulation.StateHandlers
{
    public abstract class GoingToStateHandler : ICitizenStateHandler
    {
        protected readonly ICitizenMovementService _movementService;
        protected readonly IBuildingRegistry _buildingRegistry;

        public GoingToStateHandler(ICitizenMovementService movementService, IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
            _movementService = movementService;
        }
        protected void MoveTo(Citizen citizen, MapObject target, SimulationTime time)
        {
            var (placement, found) = _buildingRegistry.TryGetPlacement(target);

            if (!found || placement == null)
            {
                citizen.State = CitizenState.Idle;
                citizen.CurrentPath.Clear();
                return;
            }

            var pos = placement.Value.Position;
            _movementService.Move(citizen, pos, time);
        }
        protected void MoveTo(Citizen citizen, Position target, SimulationTime time)
        {
            _movementService.Move(citizen, target, time);
        }

        public abstract bool CanHandle(CitizenState state);
        public abstract void Update(Citizen citizen, SimulationTime time);
    }
}
