using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;
using Domain.Enums;
using Services.Interfaces;

// Smirnov
namespace Services.Utilities
{
    public class UtilityService : IUtilityService
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<Building, Dictionary<UtilityType, int>> _brokenUtilities = new();

        public void SimulateUtilitiesBreakdown(int currentTick, List<Building> buildings)
        {
            var residentialBuildings = buildings.Where(b => b is Domain.Buildings.ResidentialBuilding).ToList();

            foreach (var building in residentialBuildings)
            {
                if (_random.Next(100) < 15) // 15% шанс для каждого здания
                {
                    var brokenUtility = (UtilityType)_random.Next(4);

                    BreakUtility(building, brokenUtility, currentTick);
                }
            }
        }

        public void BreakUtility(Building building, UtilityType utilityType, int currentTick)
        {
            building.BreakUtility(utilityType);

            if (!_brokenUtilities.ContainsKey(building))
                _brokenUtilities[building] = new Dictionary<UtilityType, int>();

            _brokenUtilities[building][utilityType] = currentTick;
        }

        public void FixUtility(Building building, UtilityType utilityType)
        {
            building.FixUtility(utilityType);

            if (_brokenUtilities.ContainsKey(building))
            {
                _brokenUtilities[building].Remove(utilityType);
                if (!_brokenUtilities[building].Any())
                    _brokenUtilities.Remove(building);
            }
        }

        public Dictionary<UtilityType, int> GetBrokenUtilities(Building building)
        {
            return _brokenUtilities.ContainsKey(building)
                ? _brokenUtilities[building]
                : new Dictionary<UtilityType, int>();
        }
    }
}
