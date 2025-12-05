using Domain.Map;
using Services.NavigationMap;

namespace Services.PathFind
{
    public class AStarPathFinder : IPathFinder
    {
        private readonly INavigationMap _navigationMap;
        public AStarPathFinder(INavigationMap navigationMap)
        {
            _navigationMap = navigationMap;
        }
     
        public List<Position> FindPath(Position start, Position goal, bool roadsOnly = false)
        {
            var result = new List<Position>();

            if (start.Equals(goal))
                return result;

            if (!_navigationMap.IsWalkable(goal, goal, roadsOnly))
                return result;

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
                    return ReconstructPathList(cameFrom, current);

                closedSet.Add(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!_navigationMap.IsWalkable(neighbor, goal, roadsOnly) || closedSet.Contains(neighbor))
                        continue;

                    int tentativeG = gScore[current] + _navigationMap.GetTileCost(neighbor);

                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);

                        if (!openSet.UnorderedItems.Any(x => x.Element.Position.Equals(neighbor)))
                            openSet.Enqueue(new Node(neighbor, fScore[neighbor]), fScore[neighbor]);
                    }
                }
            }

            return result;
        }

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
        private int HeuristicCostEstimate(Position from, Position to)
            => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

        private List<Position> ReconstructPathList(Dictionary<Position, Position> cameFrom, Position current)
        {
            var totalPath = new List<Position>();
            totalPath.Add(current);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }

            totalPath.Reverse();
            totalPath.RemoveAt(0);

            return totalPath;
        }

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
