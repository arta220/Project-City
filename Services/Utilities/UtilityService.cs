using Domain.Buildings;
using Domain.Enums;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // Статистика по типам коммунальных систем
        private readonly Dictionary<UtilityType, int> _totalBreakdowns
            = Enum.GetValues(typeof(UtilityType))
                  .Cast<UtilityType>()
                  .ToDictionary(u => u, u => 0);

        private readonly Dictionary<UtilityType, int> _totalRepairs
            = Enum.GetValues(typeof(UtilityType))
                  .Cast<UtilityType>()
                  .ToDictionary(u => u, u => 0);

        private readonly UtilityStatistics _statistics = new();

        /// <summary>
        /// Имитация поломок коммунальных систем для жилых домов
        /// </summary>
        public void SimulateUtilitiesBreakdown(int currentTick, List<ResidentialBuilding> residentialBuildings)
        {
            foreach (var building in residentialBuildings)
            {
                if (_random.Next(100) < 5) // 5% шанс поломки
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

        /// <summary>
        /// Чинит указанную систему в жилом доме
        /// </summary>
        public void FixUtility(ResidentialBuilding building, UtilityType utilityType)
        {
            building.Utilities.FixUtility(utilityType);

            if (_brokenUtilities.TryGetValue(building, out var dict))
            {
                dict.Remove(utilityType);
                if (dict.Count == 0)
                    _brokenUtilities.Remove(building);
            }

            RecordRepair(utilityType);
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

        private void RecordBreakdown(UtilityType utilityType) => _totalBreakdowns[utilityType]++;
        private void RecordRepair(UtilityType utilityType) => _totalRepairs[utilityType]++;

        private void UpdateStatistics(int currentTick)
        {
            foreach (UtilityType utilityType in Enum.GetValues(typeof(UtilityType)))
            {
                var dataPoint = new UtilityDataPoint(
                    currentTick,
                    _totalBreakdowns[utilityType],
                    _totalRepairs[utilityType]
                );

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

            var totalRepairs = _totalRepairs.Values.Sum();
            _statistics.RepairHistory.Add(new UtilityDataPoint(currentTick, 0, totalRepairs));
        }

        public UtilityStatistics GetStatistics() => _statistics;
    }
}
