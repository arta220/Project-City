using Domain.Common.Base.MovingEntities;
using Domain.Map;

namespace Domain.Transports.Sea
{
    public class Ship : MovingEntity
    {
        public Ship(
            float speed,
            INavigationProfile navigationProfile)
            : base(new Area(1, 1), speed)
        {
            NavigationProfile = navigationProfile;
        }
    }
}