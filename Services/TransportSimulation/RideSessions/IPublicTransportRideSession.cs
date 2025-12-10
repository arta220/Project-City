using Domain.Common.Base;
using Domain.Map;

namespace Services.TransportSimulation.RideSessions
{
    /// <summary>
    /// Сессия поездки водителя общественного транспорта.
    /// Транспорт движется от остановки к остановке по маршруту.
    /// </summary>
    public interface IPublicTransportRideSession
    {
        /// <summary>
        /// Общественный транспорт
        /// </summary>
        Transport Transport { get; }

        /// <summary>
        /// Список остановок на маршруте
        /// </summary>
        List<Position> Stops { get; }

        /// <summary>
        /// Индекс текущей остановки
        /// </summary>
        int CurrentStopIndex { get; }

        /// <summary>
        /// Текущая целевая остановка
        /// </summary>
        Position CurrentDestination { get; }

        /// <summary>
        /// Флаг завершения маршрута
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Обновляет состояние сессии поездки
        /// </summary>
        void Update(Domain.Common.Time.SimulationTime time);
    }
}

