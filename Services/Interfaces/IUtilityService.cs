using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;
using Domain.Enums;


// Smirnov
namespace Services.Interfaces
{
    public interface IUtilityService
    {
        void SimulateUtilitiesBreakdown(int currentTick, List<Building> buildings);
        void FixUtility(Building building, UtilityType utilityType);
        Dictionary<UtilityType, int> GetBrokenUtilities(Building building);

        UtilityStatistics GetStatistics();
        void RecordBreakdown(UtilityType utilityType);
        void RecordRepair(UtilityType utilityType);
    }

    public class UtilityStatistics
    {
        public List<UtilityDataPoint> ElectricityHistory { get; set; } = new();
        public List<UtilityDataPoint> WaterHistory { get; set; } = new();
        public List<UtilityDataPoint> GasHistory { get; set; } = new();
        public List<UtilityDataPoint> WasteHistory { get; set; } = new();
        public List<UtilityDataPoint> RepairHistory { get; set; } = new();
    }

    public class UtilityDataPoint
    {
        public int Tick { get; set; }
        public int BreakdownCount { get; set; }
        public int RepairCount { get; set; }

        public UtilityDataPoint(int tick, int breakdownCount, int repairCount)
        {
            Tick = tick;
            BreakdownCount = breakdownCount;
            RepairCount = repairCount;
        }
    }
}
