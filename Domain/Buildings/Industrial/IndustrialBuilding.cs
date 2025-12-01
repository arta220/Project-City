using Domain.Common.Base;
using Domain.Map;

namespace Domain.Buildings
{
    public class IndustrialBuilding : Building
    {
        public IndustrialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
        }
    }
}
