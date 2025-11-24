using Domain.Base;
using Domain.Map;

namespace Domain.Buildings
{
    public class ResidentialBuilding : Building
    {
        public ResidentialBuilding(
            int floors,
            int maxOccupancy,
            Area area)
            : base(floors, maxOccupancy, area)
        {
        }

        public override Building Clone()
        {
            var clonedArea = new Area(Area.Width, Area.Height);

            return new ResidentialBuilding(Floors, MaxOccupancy, clonedArea);
        }
    }
}
