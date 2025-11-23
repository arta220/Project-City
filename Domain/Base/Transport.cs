namespace Domain.Base
{
    public abstract class Transport
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public float Speed { get; set; }

        public Transport(string name, int capacity, float speed)
        {
            Name = name;
            Capacity = capacity;
            Speed = speed;
        }

        public abstract void Move();
    }
}
