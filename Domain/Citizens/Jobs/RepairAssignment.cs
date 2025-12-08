using Domain.Buildings.Residential;
using Domain.Common.Enums;
using Domain.Map;

namespace Domain.Citizens.Jobs
{
    public class RepairAssignment
    {
        public ResidentialBuilding Building { get; }
        public UtilityType UtilityType { get; }
        public Position RepairPosition { get; }

        public RepairAssignment(
            ResidentialBuilding building,
            UtilityType utilityType,
            Position repairPosition)
        {
            Building = building;
            UtilityType = utilityType;
            RepairPosition = repairPosition;
        }
    }
}