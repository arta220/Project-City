using Domain.Buildings;
using Domain.Enums;

// Smirnov
namespace Services.Interfaces
{
    public interface IUtilityService
    {
        /// <summary>
        /// Имитация поломок коммунальных систем только для жилых домов.
        /// </summary>
        void SimulateUtilitiesBreakdown(int currentTick, List<ResidentialBuilding> residentialBuildings);

        /// <summary>
        /// Починка определённой системы в жилом доме.
        /// </summary>
        void FixUtility(ResidentialBuilding building, UtilityType utilityType);

        /// <summary>
        /// Получение всех сломанных систем жилого дома.
        /// </summary>
        Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building);
    }
}
