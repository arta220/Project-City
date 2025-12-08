using Domain.Common.Base.MovingEntities;
using Domain.Map;
using Services.EntityMovement.PathFind;

namespace Services.PathFind
{
    public class AStarPathFinder : IPathFinder
    {
        public IEnumerable<Position>? FindPath(Position from, Position to, INavigationProfile profile)
        {
            var result = new List<Position>();

            if (from.Equals(to))
                return result;

            var openSet = new PriorityQueue<Node, int>();
            var closedSet = new HashSet<Position>();
            var cameFrom = new Dictionary<Position, Position>();
            var gScore = new Dictionary<Position, int> { [from] = 0 };
            var fScore = new Dictionary<Position, int> { [from] = HeuristicCostEstimate(from, to) };

            openSet.Enqueue(new Node(from, fScore[from]), fScore[from]);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue().Position;

                if (current.Equals(to))
                    return ReconstructPath(cameFrom, current);

                closedSet.Add(current);
                foreach (var neighbor in current.GetNeighbors())
                {
                    // 1. Клетка вне карты или нельзя войти — пропускаем
                    if (!profile.CanEnter(neighbor) || closedSet.Contains(neighbor))
                        continue;

                    int tentativeG = gScore[current] + profile.GetTileCost(neighbor);

                    // 2. Если клетки ещё нет или нашли более дешёвый путь
                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + HeuristicCostEstimate(neighbor, to);

                        // 3. Проверка: если соседа нет в openSet — добавляем
                        if (!openSet.UnorderedItems.Any(x => x.Element.Position == neighbor))
                        {
                            openSet.Enqueue(new Node(neighbor, fScore[neighbor]), fScore[neighbor]);
                        }
                    }
                }

            }

            return result;
        }


        private IEnumerable<Position> GetNeighbors(Position pos) => pos.GetNeighbors();

        private int HeuristicCostEstimate(Position from, Position to)
            => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

        private List<Position> ReconstructPath(Dictionary<Position, Position> cameFrom, Position current)
        {
            var path = new List<Position> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();
            path.RemoveAt(0);
            return path;
        }

        private readonly struct Node : IComparable<Node>
        {
            public Position Position { get; }
            public int FScore { get; }
            public Node(Position position, int fScore) => (Position, FScore) = (position, fScore);
            public int CompareTo(Node other) => FScore.CompareTo(other.FScore);
        }
    }
}
