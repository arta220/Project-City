using Domain.Citizens;
using Domain.Common.Base;
using Domain.Map;
using Domain.Transports.States;
using System.Collections.Generic;

namespace Domain.Transports.Ground
{
    /// <summary>
    /// Личный автомобиль жителя.
    /// Хранит текущую позицию, цель и путь в виде очереди позиций.
    /// </summary>
    public class PersonalCar : Transport
    {
        /// <summary>
        /// Текущая позиция машины на карте.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Домашняя позиция машины (где она стоит, когда "дома").
        /// </summary>
        public Position HomePosition { get; set; }

        /// <summary>
        /// Позиция работы (куда машина везёт владельца).
        /// </summary>
        public Position WorkPosition { get; set; }

        /// <summary>
        /// Владелец машины (житель).
        /// </summary>
        public Citizen? Owner { get; set; }

        /// <summary>
        /// Целевая позиция, к которой едет машина.
        /// </summary>
        public Position TargetPosition { get; set; }

        /// <summary>
        /// Текущее состояние машины.
        /// </summary>
        public TransportState State { get; set; }

        /// <summary>
        /// Количество тиков, которое машина уже простояла у работы.
        /// </summary>
        public int TicksAtWork { get; set; }

        /// <summary>
        /// Текущий путь машины как очередь клеток.
        /// </summary>
        public Queue<Position> CurrentPath { get; set; } = new Queue<Position>();

        public PersonalCar(string name, int capacity, float speed, Position startPosition)
            : base(name, capacity, speed)
        {
            Position = startPosition;
            HomePosition = startPosition;
            State = TransportState.IdleAtHome;
            TicksAtWork = 0;
        }

        /// <summary>
        /// Базовый метод движения. Фактическое перемещение выполняется сервисом движения.
        /// </summary>
        public override void Move()
        {
            // Движение реализуется через TransportMovementService.
        }
    }
}
