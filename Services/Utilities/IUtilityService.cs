using Domain.Base;
using Domain.Buildings.Residential;
using Domain.Common.Enums;
using Services.Common;

namespace Services.Utilities
{
    public interface IUtilityService : IUpdatable
    {
        void FixUtility(ResidentialBuilding building, UtilityType utilityType);
        void FixAllUtilities(ResidentialBuilding building);
        Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building);

        /// <summary>
        /// Принудительно ломает коммуналку для тестирования
        /// </summary>
        void BreakUtilityForTesting(ResidentialBuilding building, UtilityType utilityType, int currentTick);

        UtilityStatistics GetStatistics();
    }
}
