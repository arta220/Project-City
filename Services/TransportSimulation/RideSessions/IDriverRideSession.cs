using Domain.Citizens;
using Domain.Common.Base;
using Domain.Map;

namespace Services.TransportSimulation.RideSessions
{
    /// <summary>
    /// Сессия поездки водителя на личном транспорте.
    /// Содержит информацию о водителе, транспорте и конечной точке поездки.
    /// </summary>
    public interface IDriverRideSession
    {
        /// <summary>
        /// Водитель, управляющий транспортом
        /// </summary>
        Citizen Driver { get; }

        /// <summary>
        /// Транспорт, на котором едет водитель
        /// </summary>
        Transport Transport { get; }

        /// <summary>
        /// Конечная точка поездки
        /// </summary>
        Position Destination { get; }

        /// <summary>
        /// Флаг завершения поездки
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Обновляет состояние сессии поездки
        /// </summary>
        void Update(Domain.Common.Time.SimulationTime time);
    }
}

