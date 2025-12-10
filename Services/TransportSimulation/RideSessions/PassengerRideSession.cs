using Domain.Citizens;
using Domain.Map;

namespace Services.TransportSimulation.RideSessions
{
    /// <summary>
    /// Реализация сессии поездки пассажира в общественном транспорте.
    /// Пассажир указывает остановку выхода и уведомляется при достижении цели.
    /// </summary>
    public class PassengerRideSession : IPassengerRideSession
    {
        public Citizen Passenger { get; }
        public Position ExitStop { get; }
        public bool HasReachedDestination { get; private set; }

        public PassengerRideSession(Citizen passenger, Position exitStop)
        {
            Passenger = passenger ?? throw new ArgumentNullException(nameof(passenger));
            ExitStop = exitStop;
            HasReachedDestination = false;
        }

        /// <summary>
        /// Проверяет, достиг ли пассажир своей остановки.
        /// Вызывается каждый тик для проверки текущей позиции транспорта.
        /// </summary>
        /// <param name="currentTransportPosition">Текущая позиция транспорта</param>
        public void CheckDestination(Position currentTransportPosition)
        {
            if (HasReachedDestination)
                return;

            // Проверяем, достиг ли транспорт целевой остановки
            if (currentTransportPosition.Equals(ExitStop))
            {
                HasReachedDestination = true;
            }
        }
    }
}



