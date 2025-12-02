using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Transports.States;

namespace Services.TransportSimulation.StateHandlers
{
    public class DrivingHomeStateHandler : ITransportStateHandler
    {
        private readonly TransportMovementService _movementService;

        public DrivingHomeStateHandler(TransportMovementService movementService)
        {
            _movementService = movementService;
        }

        public bool CanHandle(TransportState state) => state == TransportState.DrivingHome;

        public void Update(Transport transport, SimulationTime time)
        {
            if (transport.TargetPosition == null) return;

            _movementService.Move(transport, transport.TargetPosition);

            if (transport.Position.Equals(transport.TargetPosition))
            {
                transport.State = TransportState.IdleAtHome;
            }
        }
    }
}
