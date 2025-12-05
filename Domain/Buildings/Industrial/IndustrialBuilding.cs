using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public class IndustrialBuilding : Building
    {
        public IndustrialBuildingType Type { get; }

        public IndustrialBuilding(int floors, int maxOccupancy, Area area, IndustrialBuildingType type)
            : base(floors, maxOccupancy, area)
        {
            Type = type;
        }
    }
}
