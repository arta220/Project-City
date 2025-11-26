using Domain.Map;

namespace Services.NavigationMap
{
    public interface INavigationMap
    {
        bool IsWalkable(Position position);
        int GetTileCost(Position position);
    }
}
