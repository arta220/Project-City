using Domain.Map;
using Domain.Common.Base;

namespace Domain.Transports.Ground
{
    public class Taxi : Transport
    {
        public Taxi(Area area, float speed) : base(area, speed)
        {
            Type = TransportType.Taxi;
            Capacity = 4;
        }
    }
}
