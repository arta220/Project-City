using Domain.Common.Base.MovingEntities;
using Domain.Map;

namespace Services.EntityMovement.PathFind
{
    public interface IPathFinder
    {
        IEnumerable<Position>? FindPath(
            Position from,
            Position to,
            INavigationProfile profile);
    }
}
