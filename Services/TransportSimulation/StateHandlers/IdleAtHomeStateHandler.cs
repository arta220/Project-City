using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Transports.States;

namespace Services.TransportSimulation.StateHandlers
{
    public class IdleAtHomeStateHandler : ITransportStateHandler
    {
        public bool CanHandle(TransportState state) => state == TransportState.IdleAtHome;

        public void Update(Transport transport, SimulationTime time)
        {
            if (transport.TargetPosition != null)
            {
                //transport.State = TransportState.DrivingToWork;
            }
        }
    }
}
