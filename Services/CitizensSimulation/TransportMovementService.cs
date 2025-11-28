using Domain.Map;
using Domain.Transports.Ground;
using Services.PathFind;

namespace Services.CitizensSimulation
{
    /// <summary>
    /// Сервис, отвечающий за перемещение личного транспорта (машин) по карте.
    /// Логика аналогична MovementService для граждан.
    /// </summary>
    public class TransportMovementService
    {
        private readonly IPathFinder _pathFinder;

        public TransportMovementService(IPathFinder pathFinder)
        {
            _pathFinder = pathFinder;
        }

        /// <summary>
        /// Перемещает машину к указанной позиции.
        /// Путь пересчитывается только при изменении цели или отсутствии текущего пути.
        /// </summary>
        public void Move(PersonalCar car, Position target)
        {
            if (car.CurrentPath.Count == 0 || car.TargetPosition != target)
            {
                car.TargetPosition = target;
                _pathFinder.FindPath(car.Position, car.TargetPosition, car.CurrentPath);
            }

            if (car.CurrentPath.Count > 0)
            {
                car.Position = car.CurrentPath.Dequeue();
            }
        }
    }
}
