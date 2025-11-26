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

        public Queue<Position> FindPath(Position start, Position goal)
        {
            var queue = new Queue<Position>();
            queue.Enqueue(new Position(10, 11));
            queue.Enqueue(new Position(10, 12));
            queue.Enqueue(new Position(10, 13));
            return queue;
        }
    }
}
