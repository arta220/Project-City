using Domain.Map;

namespace Domain.Base
{
    public abstract class Building : MapObject
    {
        public int Floors { get; }
        public int MaxOccupancy { get; }

        protected Building(int floors, int maxOccupancy, Area area) : base(area)
        {
            Floors = floors;
            MaxOccupancy = maxOccupancy;
        }
    }
}
