using System.Collections.Generic;

namespace Services.JewelryProduction
{
    /// <summary>
    /// Точка данных для статистики ювелирного производства
    /// </summary>
    public class ProductionDataPoint
    {
        public int Tick { get; set; }
        public int TotalProduction { get; set; }
        public int MaterialsUsed { get; set; }
        
        // Категории изделий
        public int RingsProduction { get; set; }
        public int NecklacesProduction { get; set; }
        public int BraceletsProduction { get; set; }
        public int EarringsProduction { get; set; }
        public int PendantsProduction { get; set; }
        public int PremiumProduction { get; set; } // Премиум изделия
        public int ExclusiveProduction { get; set; } // Эксклюзивные изделия

        public ProductionDataPoint(int tick, int totalProduction, int materialsUsed,
            int ringsProduction, int necklacesProduction, int braceletsProduction,
            int earringsProduction, int pendantsProduction, int premiumProduction, int exclusiveProduction)
        {
            Tick = tick;
            TotalProduction = totalProduction;
            MaterialsUsed = materialsUsed;
            RingsProduction = ringsProduction;
            NecklacesProduction = necklacesProduction;
            BraceletsProduction = braceletsProduction;
            EarringsProduction = earringsProduction;
            PendantsProduction = pendantsProduction;
            PremiumProduction = premiumProduction;
            ExclusiveProduction = exclusiveProduction;
        }
    }

    /// <summary>
    /// Статистика ювелирного производства
    /// </summary>
    public class JewelryProductionStatistics
    {
        public List<ProductionDataPoint> ProductionHistory { get; set; } = new();
    }
}
