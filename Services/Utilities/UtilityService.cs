using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;
using Domain.Buildings;
using Domain.Enums;
using Services.Interfaces;

// Smirnov
namespace Services.Utilities
{
    public class UtilityService : IUtilityService
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<Building, Dictionary<UtilityType, int>> _brokenUtilities = new();

        private readonly UtilityStatistics _statistics = new();
        private readonly Dictionary<UtilityType, int> _totalBreakdowns = new();
        private readonly Dictionary<UtilityType, int> _totalRepairs = new();

        public UtilityService()
        {
            // Инициализация счетчиков
            foreach (UtilityType utilityType in Enum.GetValues(typeof(UtilityType)))
            {
                _totalBreakdowns[utilityType] = 0;
                _totalRepairs[utilityType] = 0;
            }
        }

        public void SimulateUtilitiesBreakdown(int currentTick, List<Building> buildings)
        {
            var residentialBuildings = buildings.Where(b => b is Domain.Buildings.ResidentialBuilding).ToList();

            foreach (var building in residentialBuildings)
            {
                if (_random.Next(100) < 5) // 15% шанс для каждого здания
                {
                    var brokenUtility = (UtilityType)_random.Next(4);

                    BreakUtility(building, brokenUtility, currentTick);

                    RecordBreakdown(brokenUtility);
                    UpdateStatistics(currentTick);
                }
            }
        }

        public void BreakUtility(Building building, UtilityType utilityType, int currentTick)
        {
            if (building is ResidentialBuilding residentialBuilding)
            {
                residentialBuilding.BreakUtility(utilityType);

                if (!_brokenUtilities.ContainsKey(building))
                    _brokenUtilities[building] = new Dictionary<UtilityType, int>();

                _brokenUtilities[building][utilityType] = currentTick;
            }
        }

        public void FixUtility(Building building, UtilityType utilityType)
        {
            if (building is ResidentialBuilding residentialBuilding)
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

        public Dictionary<UtilityType, int> GetBrokenUtilities(Building building)
        {
            return _brokenUtilities.ContainsKey(building)
                ? _brokenUtilities[building]
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
