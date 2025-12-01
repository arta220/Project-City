using Domain.Common.Base;
using Domain.Map;

namespace Domain.Ground
{
    public class Taxi : Transport
    {
        public Taxi(Area area, string name, int capacity, float speed) : base(area, name, capacity, speed)
        {
        }
    }
}
