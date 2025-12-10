using Domain.Common.Base.MovingEntities;
using Domain.Map;

public class FakeNavigationProfile : INavigationProfile
{
    public bool CanEnter(Position pos) => true;
    public int GetTileCost(Position pos) => 1;
}

