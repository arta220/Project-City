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

            switch (citizen.PersonalCar.State)
            {
                case TransportState.ParkedAtWork:
                    citizen.State = CitizenState.GoingToWork;
                    break;
                case TransportState.IdleAtHome:
                    citizen.State = CitizenState.Idle;
                    break;
            }
        }
    }
}
