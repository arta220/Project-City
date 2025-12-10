using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.EntityMovement.Service;
using Services.TransportSimulation.RideSessions;

namespace Services.CitizensSimulation.Tasks
{
    /// <summary>
    /// Задача управления автобусом по маршруту
    /// </summary>
    public class DriveBusRouteTask : ICitizenTask
    {
        private readonly IPublicTransportRideSession _session;
        private readonly IEntityMovementService _movementService;

        public bool IsCompleted => _session.IsCompleted;

        public DriveBusRouteTask(IPublicTransportRideSession session, IEntityMovementService movementService)
        {
            _session = session;
            _movementService = movementService;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            // Обновляем сессию поездки
            _session.Update(time);

            // Синхронизируем позицию водителя с автобусом
            if (citizen.CurrentTransport != null)
            {
                citizen.Position = citizen.CurrentTransport.Position;
            }

            // Проверяем пассажиров на текущей остановке
            if (_session.Transport.HasReachedCurrentTarget())
            {
                HandlePassengersAtStop(_session.Transport, _session.CurrentDestination);
            }
        }

        private void HandlePassengersAtStop(Transport bus, Position stop)
        {
            // Пассажиры, которые хотят выйти на этой остановке
            var exitingPassengers = bus.Passengers
                .Where(p =>
                {
                    // Временная логика для тестирования:
                    // Каждый 3-й пассажир хочет выйти на текущей остановке
                    return bus.Passengers.IndexOf(p) % 3 == 0;
                })
                .ToList();

            foreach (var passenger in exitingPassengers)
            {
                bus.Passengers.Remove(passenger);
                passenger.CurrentTransport = null;
                passenger.Position = stop;
            }

            // Пассажиры, которые хотят войти
            // (нужно реализовать логику ожидания на остановке)
        }
    }
}
