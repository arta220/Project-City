using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Transports.States;

namespace Services.TransportSimulation.StateHandlers
{
    public class ParkedAtWorkStateHandler : ITransportStateHandler
    {
        public bool CanHandle(TransportState state) => state == TransportState.ParkedAtWork;

        public void Update(Transport transport, SimulationTime time)
        {
            transport.State = TransportState.DrivingHome;
        }
    }
}
