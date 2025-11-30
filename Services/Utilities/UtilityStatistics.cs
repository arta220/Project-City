using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utilities
{
    public class UtilityStatistics
    {
        public List<UtilityDataPoint> ElectricityHistory { get; set; } = new();
        public List<UtilityDataPoint> WaterHistory { get; set; } = new();
        public List<UtilityDataPoint> GasHistory { get; set; } = new();
        public List<UtilityDataPoint> WasteHistory { get; set; } = new();
        public List<UtilityDataPoint> RepairHistory { get; set; } = new();
    }
}
