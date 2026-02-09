using Domain.Map;

namespace Domain.Common.Base.MovingEntities
{
    public interface INavigationProfile
    {
        int GetTileCost(Position pos);
        bool CanEnter(Position pos);
    }
}
