using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Domain.Transports.States;

namespace Services.CitizensSimulation.StateHandlers
{
    public class InTransportStateHandler : ICitizenStateHandler
    {
        public bool CanHandle(CitizenState state) => state == CitizenState.InTransport;

        public void Update(Citizen citizen, SimulationTime time)
        {
            if (citizen.PersonalCar == null)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            citizen.Position = citizen.PersonalCar.Position;

            if (citizen.TargetPosition != null &&
                citizen.PersonalCar.State != TransportState.DrivingToTarget)
            {
                citizen.PersonalCar.TargetPosition = citizen.TargetPosition;
                citizen.PersonalCar.State = TransportState.DrivingToTarget;
            }

            if (citizen.PersonalCar.Position.Equals(citizen.PersonalCar.TargetPosition))
            {
                citizen.State = CitizenState.Idle;
                citizen.CurrentTransport = null;
            }
        }

    }
}
