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
                if (_random.Next(100) < 5) // 15% шанс для каждого здания
                {
                    var brokenUtility = (UtilityType)_random.Next(Enum.GetValues(typeof(UtilityType)).Length);
                    BreakUtility(building, brokenUtility, currentTick);

                    RecordBreakdown(brokenUtility);
                    UpdateStatistics(currentTick);
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
        }

        /// <summary>
        /// Чинит указанную систему в жилом доме
        /// </summary>
        public void FixUtility(ResidentialBuilding building, UtilityType utilityType)
        {
            building.Utilities.FixUtility(utilityType);

            if (_brokenUtilities.ContainsKey(building))
            {
                residentialBuilding.FixUtility(utilityType);

                if (_brokenUtilities.ContainsKey(building))
                {
                    _brokenUtilities[building].Remove(utilityType);
                    if (!_brokenUtilities[building].Any())
                        _brokenUtilities.Remove(building);
                }

                RecordRepair(utilityType);
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

        public void RecordBreakdown(UtilityType utilityType)
        {
            _totalBreakdowns[utilityType]++;
        }

        public void RecordRepair(UtilityType utilityType)
        {
            _totalRepairs[utilityType]++;
        }

        private void UpdateStatistics(int currentTick)
        {
            // Обновляем историю для каждого типа коммунальных услуг
            foreach (UtilityType utilityType in Enum.GetValues(typeof(UtilityType)))
            {
                var dataPoint = new UtilityDataPoint(
                    currentTick,
                    _totalBreakdowns[utilityType],
                    _totalRepairs[utilityType]
                );

                // Добавляем в соответствующую историю
                switch (utilityType)
                {
                    case UtilityType.Electricity:
                        _statistics.ElectricityHistory.Add(dataPoint);
                        break;
                    case UtilityType.Water:
                        _statistics.WaterHistory.Add(dataPoint);
                        break;
                    case UtilityType.Gas:
                        _statistics.GasHistory.Add(dataPoint);
                        break;
                    case UtilityType.Waste:
                        _statistics.WasteHistory.Add(dataPoint);
                        break;
                }
            }

            // Также обновляем общую историю починок
            var totalRepairs = _totalRepairs.Values.Sum();
            _statistics.RepairHistory.Add(new UtilityDataPoint(currentTick, 0, totalRepairs));
        }

        public UtilityStatistics GetStatistics()
        {
            return _statistics;
        }
    }
}
