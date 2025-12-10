using Domain.Citizens;
using Domain.Map;

namespace Services.TransportSimulation.RideSessions
{
    /// <summary>
    /// Сессия поездки пассажира в общественном транспорте.
    /// Пассажир указывает остановку выхода и уведомляется при достижении цели.
    /// </summary>
    public interface IPassengerRideSession
    {
        /// <summary>
        /// Пассажир
        /// </summary>
        Citizen Passenger { get; }

        /// <summary>
        /// Позиция остановки, на которой пассажир хочет выйти
        /// </summary>
        Position ExitStop { get; }

        /// <summary>
        /// Флаг достижения целевой остановки
        /// </summary>
        bool HasReachedDestination { get; }

        /// <summary>
        /// Проверяет, достиг ли пассажир своей остановки
        /// </summary>
        /// <param name="currentTransportPosition">Текущая позиция транспорта</param>
        void CheckDestination(Position currentTransportPosition);
    }
}



