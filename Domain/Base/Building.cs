using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Map;

namespace Domain.Base
{
    public abstract class Building : MapObject
    {
        public int Floors { get; }
        public int MaxOccupancy { get; }
        public Position Position => Area.Position;

        protected Building(int floors, int maxOccupancy, Area area) : base(area)
        {
            Floors = floors;
            MaxOccupancy = maxOccupancy;
        }

        public abstract Building Clone();
    }

}
