using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Transports.States;
using Services.BuildingRegistry;

namespace Services.TransportSimulation.StateHandlers
{
    /// <summary>
    /// Обработчик состояния "машина припаркована на работе".
    /// Простая логика: высадить всех пассажиров у их работы и оставить машину на месте.
    /// </summary>
    public class ParkedAtWorkStateHandler : ITransportStateHandler
    {
        private readonly IBuildingRegistry _registry;

        public ParkedAtWorkStateHandler(IBuildingRegistry registry)
        {
            _registry = registry;
        }

        public bool CanHandle(TransportState state) => state == TransportState.ParkedAtWork;

        public void Update(Transport transport, SimulationTime time)
        {
            // Высаживаем пассажиров и ставим им простые задачи: дойти до работы и работать
            foreach (var passenger in transport.Passengers.ToList())
            {
                if (passenger is not Citizen citizen)
                    continue;

                citizen.CurrentTransport = null;

                if (citizen.WorkPlace != null)
                {
                    var (placement, ok) = _registry.TryGetPlacement(citizen.WorkPlace);
                    if (ok)
                    {
                        var entrance = placement.Value.Entrance;

                        citizen.Tasks.Enqueue(new Domain.Citizens.Tasks.CitizenTask(
                            CitizenTaskType.MoveToPosition,
                            entrance));

                        citizen.Tasks.Enqueue(new Domain.Citizens.Tasks.CitizenTask(
                            CitizenTaskType.Work,
                            entrance));
                    }
                }

                transport.Passengers.Remove(passenger);
            }

            // Машина остаётся припаркованной у работы, ничего не делаем.
        }
    }
}
