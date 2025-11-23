using CitySkylines_REMAKE.Models.Enums;
using CitySkylines_REMAKE.Models.Map;

namespace Core.Models.Base
{
    // урезанный класс из предыдущего проекта
    public abstract class Building : GameObject
    {
        public int Floors { get; set; }
        public float Condition { get; set; } = 100f;
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxOccupancy { get; set; }

        /// <summary>
        /// Конструктор здания с значениями по умолчанию
        /// </summary>
        /// <param name="Floors"> кол-во этажей </param>
        /// <param name="Width"> ширина на карте (x) </param>
        /// <param name="Height"> длина на карте (y) </param>
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
