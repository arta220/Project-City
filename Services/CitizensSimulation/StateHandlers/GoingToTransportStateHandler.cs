using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Citizens.Movement;

namespace Services.CitizensSimulation.StateHandlers
{
    public class GoingToTransportStateHandler : GoingToStateHandler
    {
        public GoingToTransportStateHandler(ICitizenMovementService movementService, IBuildingRegistry buildingRegistry) : base(movementService, buildingRegistry) { }

        public override bool CanHandle(CitizenState state) => state == CitizenState.GoingToTransport;

        public override void Update(Citizen citizen, SimulationTime time) => MoveTo(citizen, citizen.CurrentTransport, time);
    }
}
