using Domain.Common.Base.MovingEntities;
using Domain.Map;

namespace Domain.Transports.Air
{
    public class Plane : MovingEntity
    {
        public Plane(
            float speed,
            INavigationProfile navigationProfile)
            : base(new Area(1, 1), speed)
        {
            NavigationProfile = navigationProfile;
        }
    }
}