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
        public int FireEquipmentProduction { get; set; }      // Производство противопожарного оборудования
        public int FireEquipmentMaterialsUsed { get; set; }   // Использовано материалов для противопожарки
        public int RoboticsProduction { get; set; }          // Производство роботов
        public int RoboticsMaterialsUsed { get; set; }       // Использовано материалов для роботов

        public ProductionDataPoint(int tick, int cardboardProduction, int packagingProduction,
            int cardboardMaterialsUsed, int packagingMaterialsUsed)
        {
            Tick = tick;
            CardboardProduction = cardboardProduction;
            PackagingProduction = packagingProduction;
            CardboardMaterialsUsed = cardboardMaterialsUsed;
            PackagingMaterialsUsed = packagingMaterialsUsed;
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
    }
}