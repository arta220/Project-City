using CitySkylines_REMAKE.Models.Enums;
using CitySkylines_REMAKE.Models.Map;

namespace Core.Models.Base
{
    public abstract class Building
    {
        public int Floors { get; set; }
        public float Condition { get; set; } = 100f;
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxOccupancy { get; set; }

        public Building(
            int Floors = 1,
            int Width = 1,
            int Height = 1
            )
        { 
            this.Floors = Floors;
            this.Width = Width;
            this.Height = Height;
        }



    }
}
