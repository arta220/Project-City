using Domain.Common.Time;
using Domain.Map;
using Services.EntityMovement.Service;
using Services.TransportSimulation.RideSessions;

namespace Services.TransportSimulation
{
    /// <summary>
    /// Сервис управления сессиями поездок транспорта.
    /// Отслеживает активные сессии поездок и обновляет их каждый тик.
    /// </summary>
    public class RideSessionService
    {
        private readonly List<IDriverRideSession> _driverSessions = new();
        private readonly List<IPublicTransportRideSession> _publicTransportSessions = new();
        private readonly List<IPassengerRideSession> _passengerSessions = new();
        private readonly IEntityMovementService _movementService;

        public RideSessionService(IEntityMovementService movementService)
        {
            _movementService = movementService ?? throw new ArgumentNullException(nameof(movementService));
        }

        /// <summary>
        /// Создает и добавляет сессию поездки водителя на личном транспорте.
        /// </summary>
        public IDriverRideSession CreateDriverRideSession(
            Domain.Citizens.Citizen driver,
            Domain.Common.Base.Transport transport,
            Position destination)
        {
            var session = new DriverRideSession(driver, transport, destination, _movementService);
            _driverSessions.Add(session);
            return session;
        }

        /// <summary>
        /// Создает и добавляет сессию поездки водителя общественного транспорта.
        /// </summary>
        public IPublicTransportRideSession CreatePublicTransportRideSession(
            Domain.Common.Base.Transport transport,
            List<Position> stops)
        {
            var session = new PublicTransportRideSession(transport, stops, _movementService);
            _publicTransportSessions.Add(session);
            return session;
        }

        /// <summary>
        /// Создает и добавляет сессию поездки пассажира в общественном транспорте.
        /// </summary>
        public IPassengerRideSession CreatePassengerRideSession(
            Domain.Citizens.Citizen passenger,
            Position exitStop)
        {
            var session = new PassengerRideSession(passenger, exitStop);
            _passengerSessions.Add(session);
            return session;
        }

        /// <summary>
        /// Обновляет все активные сессии поездок.
        /// </summary>
        public void Update(SimulationTime time)
        {
            // Обновляем сессии водителей
            foreach (var session in _driverSessions.ToList())
            {
                session.Update(time);
                
                // Удаляем завершенные сессии
                if (session.IsCompleted)
                {
                    _driverSessions.Remove(session);
                }
            }

            // Обновляем сессии общественного транспорта
            foreach (var session in _publicTransportSessions.ToList())
            {
                session.Update(time);
                
                // Проверяем пассажиров этого транспорта
                foreach (var passengerSession in _passengerSessions.ToList())
                {
                    if (passengerSession.Passenger.CurrentTransport == session.Transport)
                    {
                        passengerSession.CheckDestination(session.Transport.Position);
                    }
                }
                
                // Удаляем завершенные сессии
                if (session.IsCompleted)
                {
                    _publicTransportSessions.Remove(session);
                }
            }

            // Удаляем завершенные сессии пассажиров
            _passengerSessions.RemoveAll(s => s.HasReachedDestination);
        }

        /// <summary>
        /// Получает активную сессию поездки водителя для указанного транспорта.
        /// </summary>
        public IDriverRideSession? GetDriverSession(Domain.Common.Base.Transport transport)
        {
            return _driverSessions.FirstOrDefault(s => s.Transport == transport);
        }

        /// <summary>
        /// Получает активную сессию поездки общественного транспорта.
        /// </summary>
        public IPublicTransportRideSession? GetPublicTransportSession(Domain.Common.Base.Transport transport)
        {
            return _publicTransportSessions.FirstOrDefault(s => s.Transport == transport);
        }
    }
}



