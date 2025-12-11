using System.Collections.Generic;

namespace Services.IndustrialProduction
{
    /// <summary>
    /// Точка данных для статистики производства
    /// </summary>
    public class ProductionDataPoint
    {
        public int Tick { get; set; }

        // Существующие поля
        public int CardboardProduction { get; set; }
        public int PackagingProduction { get; set; }
        public int CardboardMaterialsUsed { get; set; }
        public int PackagingMaterialsUsed { get; set; }

        // НОВЫЕ ПОЛЯ ДЛЯ ТВОИХ 3 ЗАВОДОВ:
        public int ResourceExtractionProduction { get; set; }
        public int WoodProcessingProduction { get; set; }
        public int RecyclingProcessingProduction { get; set; }
        public int ResourceExtractionMaterialsUsed { get; set; }
        public int WoodProcessingMaterialsUsed { get; set; }
        public int RecyclingProcessingMaterialsUsed { get; set; }

        // Старый конструктор для совместимости
        public ProductionDataPoint(
            int tick,
            int cardboardProduction, int packagingProduction,
            int resourceExtractionProduction, int woodProcessingProduction, int recyclingProcessingProduction,
            int cardboardMaterialsUsed, int packagingMaterialsUsed,
            int resourceExtractionMaterialsUsed, int woodProcessingMaterialsUsed, int recyclingProcessingMaterialsUsed)
        {
            Tick = tick;
            CardboardProduction = cardboardProduction;
            PackagingProduction = packagingProduction;
            ResourceExtractionProduction = resourceExtractionProduction;
            WoodProcessingProduction = woodProcessingProduction;
            RecyclingProcessingProduction = recyclingProcessingProduction;
            CardboardMaterialsUsed = cardboardMaterialsUsed;
            PackagingMaterialsUsed = packagingMaterialsUsed;
            ResourceExtractionMaterialsUsed = resourceExtractionMaterialsUsed;
            WoodProcessingMaterialsUsed = woodProcessingMaterialsUsed;
            RecyclingProcessingMaterialsUsed = recyclingProcessingMaterialsUsed;
        }
        // Пустой конструктор для удобства
        public ProductionDataPoint()
        {
        }
    }

    

    /// <summary>
    /// Статистика производства промышленных зданий
    /// </summary>
    public class IndustrialProductionStatistics
    {
        public List<ProductionDataPoint> CardboardHistory { get; set; } = new();
        public List<ProductionDataPoint> PackagingHistory { get; set; } = new();
        public List<ProductionDataPoint> ResourceExtractionHistory { get; set; } = new();
        public List<ProductionDataPoint> WoodProcessingHistory { get; set; } = new();
        public List<ProductionDataPoint> RecyclingProcessingHistory { get; set; } = new();
    }
}

