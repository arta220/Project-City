using Domain.Base;
using Domain.Buildings.Residential;
using Domain.Common.Enums;
using Services.Common;

namespace Services.Utilities
{
    public interface IUtilityService : IUpdatable
    {
        void FixUtility(ResidentialBuilding building, UtilityType utilityType);
        Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building);

        UtilityStatistics GetStatistics();
    }
}
