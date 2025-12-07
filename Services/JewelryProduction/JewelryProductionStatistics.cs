using System.Collections.Generic;

namespace Services.JewelryProduction
{
    public class ProductionDataPoint
    {
        public int Tick { get; set; }
        public int JewelryProduction { get; set; }
        public int MaterialsUsed { get; set; }

        public ProductionDataPoint(int tick, int jewelryProduction, int materialsUsed)
        {
            Tick = tick;
            JewelryProduction = jewelryProduction;
            MaterialsUsed = materialsUsed;
        }
    }

    public class JewelryProductionStatistics
    {
        public List<ProductionDataPoint> ProductionHistory { get; set; } = new();
    }
}
