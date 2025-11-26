using Domain.Map;

namespace Services.PathFind
{
    public interface IPathFinder
    {
        Queue<Position> FindPath(Position current, Position target);
    }
}
