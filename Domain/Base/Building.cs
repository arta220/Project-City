using Domain.Map;

namespace Domain.Base
{
    public abstract class Building
    {
        public int Floors { get; }
        public int MaxOccupancy { get; }
        public Area Area { get; }
        public Position Position => Area.Position;

        protected Building(int floors, int maxOccupancy, Area area)
        {
            Floors = floors;
            MaxOccupancy = maxOccupancy;
            Area = area;
        }

        public abstract Building Clone();
    }

}
