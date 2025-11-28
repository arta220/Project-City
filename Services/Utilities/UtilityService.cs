using Domain.Buildings;
using Domain.Enums;
using Services.Interfaces;

namespace Services.Utilities
{
    /// <summary>
    /// Сервис имитации поломок коммунальных систем только в жилых домах.
    /// </summary>
    public class UtilityService : IUtilityService
    {
        private readonly Random _random = new Random();

        // Хранит сломанные коммунальные системы и тик поломки для каждого жилого дома
        private readonly Dictionary<ResidentialBuilding, Dictionary<UtilityType, int>> _brokenUtilities
            = new Dictionary<ResidentialBuilding, Dictionary<UtilityType, int>>();

        /// <summary>
        /// Имитация поломок коммунальных систем для жилых домов
        /// </summary>
        public void SimulateUtilitiesBreakdown(int currentTick, List<ResidentialBuilding> residentialBuildings)
        {
            foreach (var building in residentialBuildings)
            {
                if (_random.Next(100) < 15) // 15% шанс для каждого здания
                {
                    var brokenUtility = (UtilityType)_random.Next(Enum.GetValues(typeof(UtilityType)).Length);
                    BreakUtility(building, brokenUtility, currentTick);
                }
            }
        }

        /// <summary>
        /// Помечает систему как сломанную и сохраняет тик поломки
        /// </summary>
        private void BreakUtility(ResidentialBuilding building, UtilityType utilityType, int currentTick)
        {
            building.Utilities.BreakUtility(utilityType);

            if (!_brokenUtilities.ContainsKey(building))
                _brokenUtilities[building] = new Dictionary<UtilityType, int>();

            _brokenUtilities[building][utilityType] = currentTick;
        }

        /// <summary>
        /// Чинит указанную систему в жилом доме
        /// </summary>
        public void FixUtility(ResidentialBuilding building, UtilityType utilityType)
        {
            building.Utilities.FixUtility(utilityType);

            if (_brokenUtilities.ContainsKey(building))
            {
                _brokenUtilities[building].Remove(utilityType);
                if (!_brokenUtilities[building].Any())
                    _brokenUtilities.Remove(building);
            }
        }

        /// <summary>
        /// Возвращает словарь сломанных систем с тиком поломки
        /// </summary>
        public Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building)
        {
            return _brokenUtilities.ContainsKey(building)
                ? new Dictionary<UtilityType, int>(_brokenUtilities[building])
                : new Dictionary<UtilityType, int>();
        }
    }
}
