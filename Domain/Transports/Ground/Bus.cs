using Domain.Common.Base;
using System;

namespace Domain.Ground
{
    public class Bus : Transport
    {
        public Bus(string name, int capacity, float speed) : base(name, capacity, speed)
        {
        }

        public override void Move()
        {
            Console.WriteLine("Автобус едет по своему маршруту.");
        }
    }
}
