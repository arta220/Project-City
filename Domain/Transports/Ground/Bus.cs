using Domain.Common.Base;
using Domain.Map;
using System;

namespace Domain.Ground
{
    public class Bus : Transport
    {
        public Bus(Area area, string name, int capacity, float speed) : base(area, name, capacity, speed)
        {
        }
    }
}
