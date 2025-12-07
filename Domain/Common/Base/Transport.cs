using Domain.Citizens;
using Domain.Map;
using Domain.Transports;
using Domain.Transports.States;

namespace Domain.Common.Base
{
    public abstract class Transport : MovingEntity
    {
        protected Transport(Area area, float speed) : base(area, speed) { }

        public List<Position> Route { get; set; } = new();
        public int RouteIndex { get; set; }
        public int Capacity { get; set; }
        public List<Citizen> Passengers { get; } = new();
        public TransportType Type { get; protected set; }
        public TransportState State { get; set; }
    }
}
