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
        public int CardboardMaterialsUsed { get; set; }
        public int PackagingMaterialsUsed { get; set; }
         // Статистика сельского хозяйства
        public int AgricultureProduction { get; set; }
        public int AgricultureMaterialsUsed { get; set; }
        // Статистика рыбодобывающей отрасли
        public int FishingProduction { get; set; }
        public int FishingMaterialsUsed { get; set; }

        public ProductionDataPoint(int tick, int cardboardProduction, int packagingProduction, 
            int cardboardMaterialsUsed, int packagingMaterialsUsed)
        {
            Tick = tick;
            CardboardProduction = cardboardProduction;
            PackagingProduction = packagingProduction;
            CardboardMaterialsUsed = cardboardMaterialsUsed;
            PackagingMaterialsUsed = packagingMaterialsUsed;
        }
    }

    // Конструктор для сельского хозяйства
        public ProductionDataPoint(int tick, int agricultureProduction, int agricultureMaterialsUsed)
        {
            Tick = tick;
            AgricultureProduction = agricultureProduction;
            AgricultureMaterialsUsed = agricultureMaterialsUsed;
        }

        // Конструктор для рыбодобывающей отрасли
        public ProductionDataPoint(int tick, int fishingProduction, int fishingMaterialsUsed, bool isFishing = true)
        {
            Tick = tick;
            FishingProduction = fishingProduction;
            FishingMaterialsUsed = fishingMaterialsUsed;
        }

    /// <summary>
    /// Статистика производства промышленных зданий
    /// </summary>
    public class IndustrialProductionStatistics
    {
        public List<ProductionDataPoint> CardboardHistory { get; set; } = new();
        public List<ProductionDataPoint> PackagingHistory { get; set; } = new();
        public List<ProductionDataPoint> AgricultureHistory { get; set; } = new();
        public List<ProductionDataPoint> FishingHistory { get; set; } = new();
    }
}

