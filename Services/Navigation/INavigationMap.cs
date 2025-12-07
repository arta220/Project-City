using Domain.Map;

namespace Services.NavigationMap
{
    public interface INavigationMap
    {
        bool IsWalkable(Position p, Position goal);
        int GetTileCost(Position position);
    }
}
