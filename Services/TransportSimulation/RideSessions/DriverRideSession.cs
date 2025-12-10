using Domain.Citizens;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.EntityMovement.Service;

namespace Services.TransportSimulation.RideSessions
{
    /// <summary>
    /// Реализация сессии поездки водителя на личном транспорте.
    /// Водитель задает конечную точку, транспорт движется к ней.
    /// </summary>
    public class DriverRideSession : IDriverRideSession
    {
        private readonly IEntityMovementService _movementService;
        private bool _pathSet = false;

        public Citizen Driver { get; }
        public Transport Transport { get; }
        public Position Destination { get; }
        public bool IsCompleted { get; private set; }

        public DriverRideSession(
            Citizen driver,
            Transport transport,
            Position destination,
            IEntityMovementService movementService)
        {
            Driver = driver;
            Transport = transport;
            Destination = destination;
            _movementService = movementService;
        }

        /// <summary>
        /// Обновляет состояние сессии поездки.
        /// Устанавливает маршрут, если еще не установлен, и проверяет достижение цели.
        /// </summary>
        public void Update(SimulationTime time)
        {
            if (IsCompleted)
                return;

            // Устанавливаем маршрут при первом обновлении
            if (!_pathSet)
            {
                _movementService.SetTarget(Transport, Destination);
                _pathSet = true;
            }

            // Выполняем движение транспорта
            _movementService.PlayMovement(Transport, time);

            // Проверяем, достиг ли транспорт конечной точки
            if (Transport.Position.Equals(Destination) || 
                (Transport.CurrentPath.Count == 0 && Transport.Position.Equals(Destination)))
            {
                IsCompleted = true;
                // Уведомляем водителя о завершении поездки
                // Водитель может быть уведомлен через изменение состояния или через callback
            }
        }
    }
}

