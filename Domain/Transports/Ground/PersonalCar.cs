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
        public bool IsAtHome => State == TransportState.IdleAtHome;

        /// <summary>
        /// ѕровер€ет, может ли гражданин сесть в автомобиль
        /// </summary>
        public bool CanEnter(Citizen citizen)
        {
            // ¬ладелец всегда может сесть
            if (citizen == Owner)
                return true;

            // ѕассажир может сесть, если есть место и автомобиль в пути
            return Passengers.Count < Capacity &&
                   State == TransportState.DrivingToTarget &&
                   citizen.CurrentTransport == null;
        }

        /// <summary>
        /// —ажает гражданина в автомобиль
        /// </summary>
        public bool Enter(Citizen citizen)
        {
            if (!CanEnter(citizen))
                return false;

            Passengers.Add(citizen);
            citizen.CurrentTransport = this;

            // ≈сли это владелец, он становитс€ водителем
            if (citizen == Owner)
            {
                CurrentDriver = citizen;
            }

            return true;
        }

        /// <summary>
        /// ¬ысаживает гражданина из автомобил€
        /// </summary>
        public void Exit(Citizen citizen)
        {
            Passengers.Remove(citizen);
            citizen.CurrentTransport = null;

            // ≈сли это водитель
            if (CurrentDriver == citizen)
            {
                CurrentDriver = null;
                IsDriving = false;

                // ≈сли есть пассажиры, первый становитс€ водителем
                if (Passengers.Count > 0)
                {
                    CurrentDriver = Passengers[0];
                }
            }
        }
    }
}
