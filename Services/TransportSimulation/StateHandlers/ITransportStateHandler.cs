using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Transports.States;

namespace Services.TransportSimulation.StateHandlers
{
    public interface ITransportStateHandler
    {
        bool CanHandle(TransportState state);
        void Update(Transport transport, SimulationTime time);
    }
}
