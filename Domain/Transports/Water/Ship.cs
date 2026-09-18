using Domain.Common.Base;
using Domain.Map;

namespace Domain.Transports
{
    public class Ship : MovingEntity
    {
        public Ship(float speed)
            : base(
                area: new Area(1, 1),
                speed: speed)
        {
        }
    }
}