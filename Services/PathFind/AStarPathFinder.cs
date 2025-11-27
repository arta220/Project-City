using Domain.Map;
using Services.NavigationMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.PathFind
{
    public class AStarPathFinder : IPathFinder
    {
        private readonly INavigationMap _navigationMap;

        public AStarPathFinder(INavigationMap navigationMap)
        {
            _navigationMap = navigationMap;
        }

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
            var gScore = new Dictionary<Position, int>();
            var fScore = new Dictionary<Position, int>();

            gScore[start] = 0;
            fScore[start] = HeuristicCostEstimate(start, goal);
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

        private IEnumerable<Position> GetNeighbors(Position position)
        {
            var neighbors = new List<Position>
            {
                new Position(position.X, position.Y - 1),
                new Position(position.X, position.Y + 1),
                new Position(position.X - 1, position.Y),
                new Position(position.X + 1, position.Y)
            };

            return neighbors;
        }

        private int HeuristicCostEstimate(Position from, Position to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

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
            {
                pathQueue.Enqueue(totalPath.Pop());
            }
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

            public int CompareTo(Node other)
            {
                return FScore.CompareTo(other.FScore);
            }
        }
    }
}