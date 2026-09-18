using Domain.Common.Base;
using Domain.Map;

namespace Domain.Transports
{
    public class Plane : MovingEntity
    {
        public Plane(float speed)
            : base(
                area: new Area(1, 1),
                speed: speed)
        {
        }
    }
}