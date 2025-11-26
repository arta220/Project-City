using Domain.Base;
using Domain.Map;

namespace Domain.Buildings
{
    public class CommercialBuilding : Building
    {
        public CommercialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
        }

        public override CommercialBuilding Clone()
        {
            return new CommercialBuilding(Floors, MaxOccupancy, new Area(Area.Width, Area.Height));
        }
    }
}
