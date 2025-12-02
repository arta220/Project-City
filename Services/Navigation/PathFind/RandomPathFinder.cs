using Domain.Map;

namespace Services.PathFind
{
    /// <summary>
    /// Заглушка IPathFinder для тестирования: генерирует случайный путь.
    /// </summary>
    public class RandomPathFinder : IPathFinder
    {
        private readonly Random _random = new();

        public void FindPath(Position start, Position goal, Queue<Position> pathQueue)
        {
            pathQueue.Clear();

            var current = new Position(start.X, start.Y);

            for (int i = 0; i < 5; i++)
            {
                var next = GetRandomNeighbor(current);
                pathQueue.Enqueue(next);
                current = next;
            }

            pathQueue.Enqueue(goal);
        }
        private Position GetRandomNeighbor(Position pos)
        {
            var dirs = new List<(int dx, int dy)>
            {
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0)
            };

            var (dx, dy) = dirs[_random.Next(dirs.Count)];
            return new Position(pos.X + dx, pos.Y + dy);
        }
    }
}
