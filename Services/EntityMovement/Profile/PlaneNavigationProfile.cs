using Domain.Common.Base.MovingEntities;
using Domain.Map;

namespace Services.EntityMovement.Profile
{
    public class PlaneNavigationProfile : INavigationProfile
    {
        private readonly MapModel _mapModel;

        public PlaneNavigationProfile(MapModel mapModel)
        {
            _mapModel = mapModel;
        }

        public bool CanEnter(Position pos)
        {
            return _mapModel.IsPositionInBounds(pos);
        }

        public int GetTileCost(Position pos)
        {
            return 10;
        }
    }
}