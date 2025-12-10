using Domain.Citizens;
using Domain.Common.Base.MovingEntities;
using Domain.Map;
using Domain.Transports;
using Domain.Transports.States;

namespace Domain.Common.Base
{
    public abstract class Transport : MovingEntity
    {
        protected Transport(Area area, float speed) : base(area, speed) { }

        public List<Position> Route { get; set; } = new();
        public int RouteIndex { get; set; }
        public int Capacity { get; set; }
        public List<Citizen> Passengers { get; } = new();
        public TransportType Type { get; protected set; }
        public TransportState State { get; set; }
        public bool IsDriving { get; set; }
        public Citizen? CurrentDriver { get; set; }
        public Queue<Position> TargetQueue { get; } = new(); // Очередь целей
        public Position? CurrentTarget => TargetQueue.Count > 0 ? TargetQueue.Peek() : null;

        /// <summary>
        /// Добавляет цель в очередь
        /// </summary>
        public void AddTarget(Position target)
        {
            TargetQueue.Enqueue(target);
        }

        /// <summary>
        /// Переходит к следующей цели
        /// </summary>
        public void MoveToNextTarget()
        {
            if (TargetQueue.Count > 0)
            {
                TargetQueue.Dequeue();
            }
        }

        /// <summary>
        /// Проверяет, достиг ли текущей цели
        /// </summary>
        public bool HasReachedCurrentTarget()
        {
            if (CurrentTarget == null) return true;
            return Position.Equals(CurrentTarget.Value) ||
                   Position.GetNeighbors().Any(p => p.Equals(CurrentTarget.Value));
        }
    }
}
