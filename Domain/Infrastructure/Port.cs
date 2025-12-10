using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;

namespace Domain.Infrastructure
{
    public class Port : MapObject
    {
        public PortType Type { get; }

        public Port(Area area, PortType type) : base(area)
        {
            Type = type;
        }

     
    }
}
