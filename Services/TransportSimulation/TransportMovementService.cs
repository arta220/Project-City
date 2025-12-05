using Domain.Common.Base;
using Domain.Map;
using Services.NavigationMap;
using Services.PathFind;

namespace Services.TransportSimulation
{
    public class TransportMovementService
    {
        private readonly IPathFinder _pathFinder;
        private readonly INavigationMap _navigationMap;

        public TransportMovementService(IPathFinder pathFinder, INavigationMap navigationMap)
        {
            _pathFinder = pathFinder;
            _navigationMap = navigationMap;
        }

        public void Move(MovingEntity transport, Position target)
        {
            // Простейшая проверка "есть ли вообще дорога рядом с местом, где стоит машина"
            // Если во дворе вокруг машины нет дороги, она не выезжает.
            // Небольшой радиус (2 клетки) означает: дорога должна быть где-то рядом с домом.
            if (FindNearestRoadPosition(transport.Position, maxRadius: 2) is null)
            {
                return;
            }

            // Если цели нет или цель изменилась — пересчитываем маршрут
            if (transport.CurrentPath.Count == 0 || !transport.TargetPosition.Equals(target))
            {
                // Для машины цель – не сам вход в здание, а ближайшая дорога рядом с этим входом
                var parkingPosition = FindNearestRoadPosition(target);
                if (parkingPosition is null)
                {
                    // Если рядом с целью нет дорог, машина не поедет
                    return;
                }

                transport.TargetPosition = parkingPosition.Value;

                // Для транспорта ищем путь ТОЛЬКО по дорогам
                var path = _pathFinder.FindPath(transport.Position, transport.TargetPosition, roadsOnly: true);
                if (path == null || path.Count == 0)
                    return;

                transport.CurrentPath.Clear();
                foreach (var p in path)
                    transport.CurrentPath.Enqueue(p);
            }

            // Двигаем транспорт по уже рассчитанному пути
            if (transport.CurrentPath.Count > 0)
            {
                transport.Position = transport.CurrentPath.Dequeue();
            }
        }

        /// <summary>
        /// Ищет ближайшую клетку с дорогой вокруг указанной точки (например, входа в здание).
        /// </summary>
        private Position? FindNearestRoadPosition(Position goal, int maxRadius = 10)
        {
            // Радиус 0 означает: сначала пробуем саму клетку цели
            for (int radius = 0; radius <= maxRadius; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        // Берём только клетки на текущем "кольце" Манхэттенского радиуса
                        if (Math.Abs(dx) + Math.Abs(dy) != radius)
                            continue;

                        var candidate = new Position(goal.X + dx, goal.Y + dy);

                        // roadsOnly = true означает: это должна быть клетка с дорогой
                        if (_navigationMap.IsWalkable(candidate, candidate, roadsOnly: true))
                        {
                            return candidate;
                        }
                    }
                }
            }

            // В разумном радиусе дорог не нашли
            return null;
        }
    }
}
