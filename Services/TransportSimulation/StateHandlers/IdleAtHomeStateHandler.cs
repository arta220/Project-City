using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Transports.States;
using Services.BuildingRegistry;

namespace Services.TransportSimulation.StateHandlers
{
    /// <summary>
    /// Обработчик состояния "машина находится дома".
    /// Простая логика: высадить пассажиров у дома и оставить машину на месте.
    /// </summary>
    public class IdleAtHomeStateHandler : ITransportStateHandler
    {
        private readonly IBuildingRegistry _registry;

        public IdleAtHomeStateHandler(IBuildingRegistry registry)
        {
            _registry = registry;
        }

        public bool CanHandle(TransportState state) => state == TransportState.IdleAtHome;

        public void Update(Transport transport, SimulationTime time)
        {
            foreach (var passenger in transport.Passengers.ToList())
            {
                if (passenger is not Citizen citizen)
                    continue;

                citizen.CurrentTransport = null;

                if (citizen.Home != null)
                {
                    var (placement, ok) = _registry.TryGetPlacement(citizen.Home);
                    if (ok)
                    {
                        var entrance = placement.Value.Entrance;

                        citizen.Tasks.Enqueue(new Domain.Citizens.Tasks.CitizenTask(
                            CitizenTaskType.MoveToPosition,
                            entrance));
                    }
                }

                transport.Passengers.Remove(passenger);
            }

            // Машина остаётся дома, ничего не делаем.
        }
    }
}
