using Domain.Base;
using System;

namespace Domain.Ground
{
    public class Taxi : Transport
    {
        public Taxi(string name, int capacity, float speed) : base(name, capacity, speed)
        {
        }

        public override void Move()
        {
            Console.WriteLine("Такси везет пассажира по указанному адресу.");
        }
    }
}
