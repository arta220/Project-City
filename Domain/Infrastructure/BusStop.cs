using Domain.Citizens;
using Domain.Common.Base;
using Domain.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Infrastructure
{
    public class BusStop : MapObject
    {
        public BusStop(Area area) : base(area)
        {
        }

        public List<Citizen> WaitingPassengers { get; } = new();

        public void AddWaitingPassenger(Citizen passenger)
        {
            if (!WaitingPassengers.Contains(passenger))
            {
                WaitingPassengers.Add(passenger);
                passenger.Position = GetRandomPositionNearStop();
            }
        }

        private Position GetRandomPositionNearStop()
        {
            // Используем базовое свойство Position из MapObject
            var basePosition = base.Position; // или просто Position, если не конфликтует

            // Возвращаем позицию рядом с остановкой
            // Например, случайное смещение в пределах 1 клетки
            var random = new Random();
            int offsetX = random.Next(-1, 2); // -1, 0, или 1
            int offsetY = random.Next(-1, 2);

            return new Position(basePosition.X + offsetX, basePosition.Y + offsetY);
        }
    }
}
