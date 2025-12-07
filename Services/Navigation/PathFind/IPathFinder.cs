using Domain.Map;

namespace Services.PathFind
{
    public interface IPathFinder
    {
        List<Position> FindPath(Position current, Position target);
    }
}
