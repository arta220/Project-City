using Domain.Citizens;
using Domain.Map;
using Domain.Common.Base;
using Domain.Transports.States;

namespace Domain.Transports.Ground
{
    public class PersonalCar : Transport
    {
        public PersonalCar(Area area, float speed, Citizen owner) : base(area, speed)
        {
            Type = TransportType.Car;
            Capacity = 1;
            Owner = owner;
        }
        public Citizen Owner { get; }
    }
}
