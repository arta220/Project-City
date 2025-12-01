using Domain.Map;
using Services.NavigationMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.PathFind
{
    /// <summary>
    /// Реализация алгоритма A* для поиска пути на карте.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется для перемещения граждан по карте с учётом препятствий и стоимости плиток.
    /// - Работает совместно с <see cref="INavigationMap"/>, который определяет проходимость и стоимость плитки.
    /// 
    /// Возможные расширения:
    /// - Поддержка диагональных перемещений.
    /// - Учёт динамических препятствий (другие граждане, временные блокировки).
    /// - Использование различных эвристик для улучшения производительности.
    /// </remarks>
    public class AStarPathFinder : IPathFinder
    {
        private readonly INavigationMap _navigationMap;

        /// <summary>
        /// Инициализирует поиск пути на основе навигационной карты.
        /// </summary>
        /// <param name="navigationMap">Сервис навигации по карте.</param>
        public AStarPathFinder(INavigationMap navigationMap)
        {
            _navigationMap = navigationMap;
        }

        /// <summary>
        /// Находит путь от стартовой позиции до цели и заполняет очередь позиций.
        /// </summary>
        /// <param name="start">Начальная позиция.</param>
        /// <param name="goal">Целевая позиция.</param>
        /// <param name="pathQueue">Очередь, в которую будет записан путь.</param>
        public void FindPath(Position start, Position goal, Queue<Position> pathQueue)
        {
            pathQueue.Clear();

            if (start.Equals(goal))
                return;

            if (!_navigationMap.IsWalkable(goal, goal))
                return;

            var openSet = new PriorityQueue<Node, int>();
            var closedSet = new HashSet<Position>();
            var cameFrom = new Dictionary<Position, Position>();
            var gScore = new Dictionary<Position, int> { [start] = 0 };
            var fScore = new Dictionary<Position, int> { [start] = HeuristicCostEstimate(start, goal) };

            openSet.Enqueue(new Node(start, fScore[start]), fScore[start]);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue().Position;

                if (current.Equals(goal))
                {
                    ReconstructPath(cameFrom, current, pathQueue);
                    return;
                }

                closedSet.Add(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!_navigationMap.IsWalkable(neighbor, goal) || closedSet.Contains(neighbor))
                        continue;

                    var tentativeGScore = gScore[current] + _navigationMap.GetTileCost(neighbor);

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);

                        if (!openSet.UnorderedItems.Any(item => item.Element.Position.Equals(neighbor)))
                        {
                            openSet.Enqueue(new Node(neighbor, fScore[neighbor]), fScore[neighbor]);
                        }
                    }
                }
            }

            pathQueue.Clear();
        }

        /// <summary>
        /// Получает соседние позиции (по горизонтали и вертикали) для текущей позиции.
        /// </summary>
        /// <param name="position">Текущая позиция.</param>
        /// <returns>Список соседних позиций.</returns>
        private IEnumerable<Position> GetNeighbors(Position position)
        {
            return new List<Position>
            {
                new Position(position.X, position.Y - 1),
                new Position(position.X, position.Y + 1),
                new Position(position.X - 1, position.Y),
                new Position(position.X + 1, position.Y)
            };
        }

        /// <summary>
        /// Эвристическая оценка стоимости пути от одной позиции к другой (манхэттенское расстояние).
        /// </summary>
        private int HeuristicCostEstimate(Position from, Position to)
            => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

        /// <summary>
        /// Восстанавливает путь от цели к старту на основе словаря cameFrom и заполняет очередь.
        /// </summary>
        private void ReconstructPath(Dictionary<Position, Position> cameFrom, Position current, Queue<Position> pathQueue)
        {
            var totalPath = new Stack<Position>();
            totalPath.Push(current);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Push(current);
            }

            if (totalPath.Count > 0)
                totalPath.Pop();

            while (totalPath.Count > 0)
                pathQueue.Enqueue(totalPath.Pop());
        }

        /// <summary>
        /// Вспомогательная структура для хранения позиции и оценки пути для очереди приоритетов.
        /// </summary>
        private readonly struct Node : IComparable<Node>
        {
            public Position Position { get; }
            public int FScore { get; }

            public Node(Position position, int fScore)
            {
                Position = position;
                FScore = fScore;
            }

            public int CompareTo(Node other) => FScore.CompareTo(other.FScore);
        }
    }
}
