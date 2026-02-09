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
        public int AlcoholProduction { get; set; }
        public int CardboardMaterialsUsed { get; set; }
        public int PackagingMaterialsUsed { get; set; }
        public int FireEquipmentProduction { get; set; }      // Производство противопожарного оборудования
        public int FireEquipmentMaterialsUsed { get; set; }   // Использовано материалов для противопожарки
        public int RoboticsProduction { get; set; }          // Производство роботов
        public int RoboticsMaterialsUsed { get; set; }       // Использовано материалов для роботов

        public ProductionDataPoint(int tick, int cardboardProduction, int packagingProduction,
            int cardboardMaterialsUsed, int packagingMaterialsUsed)
        public int CosmeticsMaterialsUsed { get; set; }
        public int AlcoholMaterialsUsed { get; set; }

        public ProductionDataPoint(int tick, int cardboardProduction, int packagingProduction,
            int cosmeticsProduction, int alcoholProduction,
            int cardboardMaterialsUsed, int packagingMaterialsUsed,
            int cosmeticsMaterialsUsed, int alcoholMaterialsUsed)
        {
            Tick = tick;
            CardboardProduction = cardboardProduction;
            PackagingProduction = packagingProduction;
            CosmeticsProduction = cosmeticsProduction;
            AlcoholProduction = alcoholProduction;
            CardboardMaterialsUsed = cardboardMaterialsUsed;
            PackagingMaterialsUsed = packagingMaterialsUsed;
            CosmeticsMaterialsUsed = cosmeticsMaterialsUsed;
            AlcoholMaterialsUsed = alcoholMaterialsUsed;
        }

        public ProductionDataPoint(
            int tick,
            int cardboardProduction, int packagingProduction,
            int cardboardMaterialsUsed, int packagingMaterialsUsed,
            int fireEquipmentProduction, int fireEquipmentMaterialsUsed,
            int roboticsProduction, int roboticsMaterialsUsed)
        {
            Tick = tick;
            CardboardProduction = cardboardProduction;
            PackagingProduction = packagingProduction;
            CardboardMaterialsUsed = cardboardMaterialsUsed;
            PackagingMaterialsUsed = packagingMaterialsUsed;
            FireEquipmentProduction = fireEquipmentProduction;
            FireEquipmentMaterialsUsed = fireEquipmentMaterialsUsed;
            RoboticsProduction = roboticsProduction;
            RoboticsMaterialsUsed = roboticsMaterialsUsed;
        }
    }

    /// <summary>
    /// Статистика производства промышленных зданий
    /// </summary>
    public class IndustrialProductionStatistics
    {
        public List<ProductionDataPoint> CardboardHistory { get; set; } = new();
        public List<ProductionDataPoint> PackagingHistory { get; set; } = new();
        public List<ProductionDataPoint> FireEquipmentHistory { get; set; } = new();
        public List<ProductionDataPoint> RoboticsHistory { get; set; } = new();
        public List<ProductionDataPoint> CosmeticsHistory { get; set; } = new();
        public List<ProductionDataPoint> AlcoholHistory { get; set; } = new();
    }
}