using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Map;
using Domain.Transports;
using Services.Citizens.Scenaries;
using Services.CitizensSimulation.Tasks;
using Services.EntityMovement.Service;
using Services.Time;
using Services.TransportSimulation;

namespace Services.Citizens.Scenarios
{
    /// <summary>
    /// Сценарий для водителя общественного транспорта
    /// </summary>
    public class BusDriverScenario : ICitizenScenario
    {
        private readonly IEntityMovementService _movementService;
        private readonly RideSessionService _rideSessionService;

        public BusDriverScenario(
            IEntityMovementService movementService,
            RideSessionService rideSessionService)
        {
            _movementService = movementService;
            _rideSessionService = rideSessionService;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Сценарий может запуститься, если:
            // 1. Гражданин работает водителем автобуса
            // 2. У него есть назначенный автобус
            // 3. Он на работе (не дома)
            return citizen.Profession == CitizenProfession.BusDriver &&
                   citizen.CurrentTransport != null &&
                   citizen.CurrentTransport.Type == TransportType.Bus &&
                   citizen.State == CitizenState.Working;
        }

        public void BuildTasks(Citizen citizen)
        {
            if (citizen.CurrentTransport == null)
                return;

            var bus = citizen.CurrentTransport;

            // Получаем маршрут автобуса
            var stops = GetBusStops(bus);

            if (stops.Count == 0)
                return;

            // Создаем сессию поездки на общественном транспорте
            var session = _rideSessionService.CreatePublicTransportRideSession(bus, stops);

            // Задача вождения автобуса по маршруту
            citizen.Tasks.Enqueue(new DriveBusRouteTask(session, _movementService));
        }

        private List<Position> GetBusStops(Transport bus)
        {
            // В реальной реализации получаем остановки из маршрута
            // Здесь заглушка для примера
            return new List<Position>
            {
                new Position(10, 10),
                new Position(20, 10),
                new Position(30, 15),
                new Position(25, 20)
            };
        }
    }
}
