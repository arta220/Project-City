using Domain.Map;
using Domain.Common.Base;

namespace Domain.Transports.Ground
{
    public class Bus : Transport
    {
        public Bus(Area area, float speed) : base(area, speed)
        {
            Type = TransportType.Bus;
            Capacity = 40;
        }
    }
}
