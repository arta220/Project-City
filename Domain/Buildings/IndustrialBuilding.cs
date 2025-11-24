using Domain.Base;
using Domain.Map;

namespace Domain.Buildings
{
    public class IndustrialBuilding : Building
    {
        public IndustrialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
        }

        public override Building Clone()
        {
            return new IndustrialBuilding(Floors, MaxOccupancy, new Area(Area.Width, Area.Height));
        }
    }
}
