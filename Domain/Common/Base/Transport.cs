using Domain.Map;

namespace Domain.Common.Base
{
    public abstract class Transport : MapObject
    {
        protected Transport(Area area, string name, int capacity, float speed) : base(area)
        {
            Name = name;
            Capacity = capacity;
            Speed = speed;
        }

        public string Name { get; set; }
        public int Capacity { get; set; }
        public float Speed { get; set; }
    }
}
