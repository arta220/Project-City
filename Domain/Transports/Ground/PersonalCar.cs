using Domain.Citizens;
using Domain.Common.Base;
using Domain.Map;
using Domain.Transports.States;

namespace Domain.Transports.Ground
{
    /// <summary>
    /// Личный автомобиль жителя.
    /// Хранит текущую позицию, цель и путь в виде очереди позиций.
    /// </summary>
    public class PersonalCar : Transport
    {

        public PersonalCar(Area area, string name, int capacity, float speed, Position startPosition)
        : base(area, name, capacity, speed)
        {
            Position = startPosition;
            State = TransportState.IdleAtHome;
            TicksAtWork = 0;
        }
        /// <summary>
        /// Текущая позиция машины на карте.
        /// </summary>
        public Position Position { get; set; }

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
    }
}
