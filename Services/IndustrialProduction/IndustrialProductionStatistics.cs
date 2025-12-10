using System.Collections.Generic;

namespace Services.IndustrialProduction
{
    /// <summary>
    /// Точка данных для статистики производства
    /// </summary>
    public class ProductionDataPoint
    {
        public int Tick { get; set; }
        public int CardboardProduction { get; set; }
        public int PackagingProduction { get; set; }
        public int CosmeticsProduction { get; set; }
        public int CardboardMaterialsUsed { get; set; }
        public int PackagingMaterialsUsed { get; set; }
        public int CosmeticsMaterialsUsed { get; set; }

        public ProductionDataPoint(int tick, int cardboardProduction, int packagingProduction, int cosmeticsProduction,
            int cardboardMaterialsUsed, int packagingMaterialsUsed, int cosmeticsMaterialsUsed)
        {
            Tick = tick;
            CardboardProduction = cardboardProduction;
            PackagingProduction = packagingProduction;
            CosmeticsProduction = cosmeticsProduction;
            CardboardMaterialsUsed = cardboardMaterialsUsed;
            PackagingMaterialsUsed = packagingMaterialsUsed;
            CosmeticsMaterialsUsed = cosmeticsMaterialsUsed;
        }
    }


    /// <summary>
    /// Статистика производства промышленных зданий
    /// </summary>
    public class IndustrialProductionStatistics
    {
        public List<ProductionDataPoint> CardboardHistory { get; set; } = new();
        public List<ProductionDataPoint> PackagingHistory { get; set; } = new();
        public List<ProductionDataPoint> CosmeticsHistory { get; set; } = new();
    }
}

