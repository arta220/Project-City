using Domain.Map;
using System.Collections.Generic;

namespace Services.PathFind
{
    public interface IPathFinder
    {
        void FindPath(Position current, Position target, Queue<Position> pathQueue);
    }
}
