using System.Collections.Generic;

namespace Services.GlassProduction
{
    /// <summary>
    /// Точка данных для статистики стекольного производства
    /// </summary>
    public class ProductionDataPoint
    {
        public int Tick { get; set; }
        public int TotalProduction { get; set; }
        public int MaterialsUsed { get; set; }
        
        // Категории изделий
        public int BottlesProduction { get; set; }
        public int VasesProduction { get; set; }
        public int WindowsProduction { get; set; }
        public int MirrorsProduction { get; set; }
        public int TablewareProduction { get; set; }
        public int PremiumProduction { get; set; } // Премиум изделия
        public int ExclusiveProduction { get; set; } // Эксклюзивные изделия

        public ProductionDataPoint(int tick, int totalProduction, int materialsUsed,
            int bottlesProduction, int vasesProduction, int windowsProduction,
            int mirrorsProduction, int tablewareProduction, int premiumProduction, int exclusiveProduction)
        {
            Tick = tick;
            TotalProduction = totalProduction;
            MaterialsUsed = materialsUsed;
            BottlesProduction = bottlesProduction;
            VasesProduction = vasesProduction;
            WindowsProduction = windowsProduction;
            MirrorsProduction = mirrorsProduction;
            TablewareProduction = tablewareProduction;
            PremiumProduction = premiumProduction;
            ExclusiveProduction = exclusiveProduction;
        }
    }

    /// <summary>
    /// Статистика стекольного производства
    /// </summary>
    public class GlassProductionStatistics
    {
        public List<ProductionDataPoint> ProductionHistory { get; set; } = new();
    }
}
