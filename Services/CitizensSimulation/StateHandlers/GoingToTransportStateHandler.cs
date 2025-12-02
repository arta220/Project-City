using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Domain.Transports.States;
using Services.BuildingRegistry;
using Services.Citizens.Movement;

namespace Services.CitizensSimulation.StateHandlers
{
    public class GoingToTransportStateHandler : GoingToStateHandler
    {
        public GoingToTransportStateHandler(ICitizenMovementService movementService, IBuildingRegistry buildingRegistry) : base(movementService, buildingRegistry) { }

        public override bool CanHandle(CitizenState state) => state == CitizenState.GoingToTransport;

        public override void Update(Citizen citizen, SimulationTime time)
        {
            if (citizen.PersonalCar == null)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            MoveTo(citizen, citizen.PersonalCar.Position, time);

            if (citizen.Position.Equals(citizen.PersonalCar.Position))
            {
                citizen.CurrentTransport = citizen.PersonalCar;
                citizen.State = CitizenState.InTransport;
            }
        }

    }
}
