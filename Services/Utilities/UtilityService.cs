using Domain.Buildings.Residential;
using Domain.Common.Enums;
using Domain.Common.Time;
using Services.BuildingRegistry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Utilities
{
    public class UtilityService : IUtilityService
    {
        private readonly Random _random = new Random();
        private readonly IBuildingRegistry _buildingRegistry;

        private readonly Dictionary<ResidentialBuilding, Dictionary<UtilityType, int>> _brokenUtilities
            = new Dictionary<ResidentialBuilding, Dictionary<UtilityType, int>>();

        private readonly Dictionary<UtilityType, int> _totalBreakdowns
            = Enum.GetValues(typeof(UtilityType))
                  .Cast<UtilityType>()
                  .ToDictionary(u => u, u => 0);

        private readonly Dictionary<UtilityType, int> _totalRepairs
            = Enum.GetValues(typeof(UtilityType))
                  .Cast<UtilityType>()
                  .ToDictionary(u => u, u => 0);

        private readonly UtilityStatistics _statistics = new();

        public UtilityService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        public void Update(SimulationTime time)
        {
            var residentialBuildings = _buildingRegistry.GetBuildings<ResidentialBuilding>().ToList();

            foreach (var building in residentialBuildings)
            {
                if (_random.Next(100) < 5) // 5% шанс поломки
                {
                    var brokenUtility = (UtilityType)_random.Next(Enum.GetValues(typeof(UtilityType)).Length);
                    BreakUtility(building, brokenUtility, time.TotalTicks);
                    RecordBreakdown(brokenUtility);
                    UpdateStatistics(time.TotalTicks);
                }
            }
        }

        private void BreakUtility(ResidentialBuilding building, UtilityType utilityType, int currentTick)
        {
            building.Utilities.BreakUtility(utilityType);

            if (!_brokenUtilities.ContainsKey(building))
                _brokenUtilities[building] = new Dictionary<UtilityType, int>();

            _brokenUtilities[building][utilityType] = currentTick;
        }

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