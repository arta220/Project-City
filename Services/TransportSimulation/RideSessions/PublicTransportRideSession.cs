using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.EntityMovement.Service;

namespace Services.TransportSimulation.RideSessions
{
    /// <summary>
    /// Реализация сессии поездки водителя общественного транспорта.
    /// Транспорт движется от остановки к остановке по маршруту.
    /// </summary>
    public class PublicTransportRideSession : IPublicTransportRideSession
    {
        private readonly IEntityMovementService _movementService;
        private bool _pathSet = false;

        public Transport Transport { get; }
        public List<Position> Stops { get; }
        public int CurrentStopIndex { get; private set; }
        public Position CurrentDestination
        {
            get
            {
                if (Stops.Count == 0)
                    throw new InvalidOperationException("Остановок нет.");
                if (CurrentStopIndex >= Stops.Count)
                    return Stops[Stops.Count - 1];
                return Stops[CurrentStopIndex];
            }
        }
        public bool IsCompleted { get; private set; }

        public PublicTransportRideSession(
            Transport transport,
            List<Position> stops,
            IEntityMovementService movementService)
        {
            Transport = transport;
            Stops = stops ?? throw new ArgumentNullException(nameof(stops));
            _movementService = movementService;
            CurrentStopIndex = 0;
        }

        /// <summary>
        /// Обновляет состояние сессии поездки.
        /// Движется к текущей остановке, при достижении переходит к следующей.
        /// </summary>
        public void Update(Domain.Common.Time.SimulationTime time)
        {
            if (IsCompleted)
                return;

            // Если нет остановок, завершаем сессию
            if (Stops.Count == 0)
            {
                IsCompleted = true;
                return;
            }

            // Устанавливаем маршрут к текущей остановке, если еще не установлен
            if (!_pathSet)
            {
                _movementService.SetTarget(Transport, CurrentDestination);
                _pathSet = true;
            }

            // Выполняем движение транспорта
            _movementService.PlayMovement(Transport, time);

            // Проверяем, достиг ли транспорт текущей остановки
            if (Transport.Position.Equals(CurrentDestination) || 
                (Transport.CurrentPath.Count == 0 && Transport.Position.Equals(CurrentDestination)))
            {
                // Переходим к следующей остановке
                CurrentStopIndex++;

                // Если достигли последней остановки, завершаем сессию
                if (CurrentStopIndex >= Stops.Count)
                {
                    IsCompleted = true;
                }
                else
                {
                    // Устанавливаем маршрут к следующей остановке
                    _pathSet = false;
                    _movementService.SetTarget(Transport, CurrentDestination);
                    _pathSet = true;
                }
            }
        }
    }
}

