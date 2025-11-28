using Domain.Base;
using Domain.Buildings;
using Domain.Enums;
using Services.Utilities;

namespace Services.Interfaces
{
    public interface IUtilityService
    {
        void SimulateUtilitiesBreakdown(int currentTick, List<ResidentialBuilding> buildings);
        void FixUtility(ResidentialBuilding building, UtilityType utilityType);
        Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building);

        UtilityStatistics GetStatistics();
    }
}
