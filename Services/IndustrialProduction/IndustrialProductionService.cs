using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Common;
using System.Linq;

namespace Services.IndustrialProduction
{
    public class IndustrialProductionService : IIndustrialProductionService, IUpdatable
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly IndustrialProductionStatistics _statistics = new();

        private int _totalCardboardProduction = 0;
        private int _totalPackagingProduction = 0;
        private int _totalCardboardMaterialsUsed = 0;
        private int _totalPackagingMaterialsUsed = 0;

        // ПЕРЕМЕННЫЕ ДЛЯ СТАТИСТИКИ 
        private int _totalFireEquipmentProduction = 0;
        private int _totalRoboticsProduction = 0;
        private int _totalFireEquipmentMaterialsUsed = 0;
        private int _totalRoboticsMaterialsUsed = 0;

        public IndustrialProductionService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        public void Update(SimulationTime time)
        {
            var industrialBuildings = _buildingRegistry.GetBuildings<IndustrialBuilding>().ToList();

            int cardboardProduction = 0;
            int packagingProduction = 0;
            int cardboardMaterialsUsed = 0;
            int packagingMaterialsUsed = 0;
            int fireEquipmentProduction = 0;
            int roboticsProduction = 0;
            int fireEquipmentMaterialsUsed = 0;
            int roboticsMaterialsUsed = 0;

            foreach (var building in industrialBuildings)
            {
                // Запуск производства
                building.RunOnce();

                // Подсчёт производства картона
                var cardboardProducts = building.ProductsBank
                    .Where(kvp => IsCardboardProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);
                cardboardProduction += cardboardProducts;

                // Подсчёт производства упаковки
                var packagingProducts = building.ProductsBank
                    .Where(kvp => IsPackagingProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);
                packagingProduction += packagingProducts;

                // ПОДСЧЁТ ПРОИЗВОДСТВА ПРОТИВОПОЖАРНОГО ОБОРУДОВАНИЯ
                var fireEquipmentProducts = building.ProductsBank
                    .Where(kvp => IsFireEquipmentProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);
                fireEquipmentProduction += fireEquipmentProducts;

                // ПОДСЧЁТ ПРОИЗВОДСТВА РОБОТОВ
                var roboticsProducts = building.ProductsBank
                    .Where(kvp => IsRoboticsProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);
                roboticsProduction += roboticsProducts;

                // Подсчёт использованных материалов для картона
                var cardboardMaterials = building.MaterialsBank
                    .Where(kvp => IsCardboardMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);
                cardboardMaterialsUsed += cardboardMaterials;

                // Подсчёт использованных материалов для упаковки
                var packagingMaterials = building.MaterialsBank
                    .Where(kvp => IsPackagingMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);
                packagingMaterialsUsed += packagingMaterials;

                // ПОДСЧЁТ МАТЕРИАЛОВ ДЛЯ ПРОТИВОПОЖАРНОГО ОБОРУДОВАНИЯ
                var fireEquipmentMaterials = building.MaterialsBank
                    .Where(kvp => IsFireEquipmentMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);
                fireEquipmentMaterialsUsed += fireEquipmentMaterials;

                // ПОДСЧЁТ МАТЕРИАЛОВ ДЛЯ РОБОТОВ
                var roboticsMaterials = building.MaterialsBank
                    .Where(kvp => IsRoboticsMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);
                roboticsMaterialsUsed += roboticsMaterials;
            }

            // Обновление общей статистики
            _totalCardboardProduction += cardboardProduction;
            _totalPackagingProduction += packagingProduction;
            _totalCardboardMaterialsUsed += cardboardMaterialsUsed;
            _totalPackagingMaterialsUsed += packagingMaterialsUsed;

            // ОБНОВЛЕНИЕ СТАТИСТИКИ ДЛЯ НОВЫХ ФАБРИК
            _totalFireEquipmentProduction += fireEquipmentProduction;
            _totalRoboticsProduction += roboticsProduction;
            _totalFireEquipmentMaterialsUsed += fireEquipmentMaterialsUsed;
            _totalRoboticsMaterialsUsed += roboticsMaterialsUsed;

            // Добавление точки данных
            var dataPoint = new ProductionDataPoint(
                time.TotalTicks,
                cardboardProduction,
                packagingProduction,
                cardboardMaterialsUsed,
                packagingMaterialsUsed,
                fireEquipmentProduction,    // Новый параметр
                fireEquipmentMaterialsUsed, // Новый параметр
                roboticsProduction,         // Новый параметр
                roboticsMaterialsUsed       // Новый параметр
            );

            // Добавление в историю
            _statistics.CardboardHistory.Add(dataPoint);
            _statistics.PackagingHistory.Add(dataPoint);
            _statistics.FireEquipmentHistory.Add(dataPoint);  // Новая история
            _statistics.RoboticsHistory.Add(dataPoint);       // Новая история
        }

        public IndustrialProductionStatistics GetStatistics() => _statistics;

        private bool IsCardboardProduct(object product)
        {
            if (product is ProductType productType)
            {
                return productType == ProductType.CardboardSheets ||
                       productType == ProductType.CorrugatedCardboard ||
                       productType == ProductType.SolidCardboard ||
                       productType == ProductType.CardboardBoxes ||
                       productType == ProductType.CardboardTubes ||
                       productType == ProductType.EggPackaging ||
                       productType == ProductType.CardboardPallet ||
                       productType == ProductType.DisplayStand ||
                       productType == ProductType.ProtectivePackaging ||
                       productType == ProductType.CustomShapeCardboard;
            }
            return false;
        }

        private bool IsPackagingProduct(object product)
        {
            if (product is ProductType productType)
            {
                return productType == ProductType.CardboardBox ||
                       productType == ProductType.PlasticBottle ||
                       productType == ProductType.GlassJar ||
                       productType == ProductType.AluminiumCan ||
                       productType == ProductType.WoodenCrate ||
                       productType == ProductType.FoodContainer ||
                       productType == ProductType.ShippingBox ||
                       productType == ProductType.CosmeticBottle ||
                       productType == ProductType.PharmaceuticalPack ||
                       productType == ProductType.GiftBox;
            }
            return false;
        }

        // МЕТОДЫ ДЛЯ ПРОВЕРКИ ПРОДУКЦИИ
        private bool IsFireEquipmentProduct(object product)
        {
            if (product is ProductType productType)
            {
                return productType == ProductType.FireExtinguisher ||
                       productType == ProductType.FireHose ||
                       productType == ProductType.FireAlarmSystem ||
                       productType == ProductType.FireTruck;
            }
            return false;
        }

        private bool IsRoboticsProduct(object product)
        {
            if (product is ProductType productType)
            {
                return productType == ProductType.IndustrialRobot ||
                       productType == ProductType.RobotArm ||
                       productType == ProductType.RobotController ||
                       productType == ProductType.AutomationSystem;
            }
            return false;
        }

        private bool IsCardboardMaterial(object material)
        {
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.WoodChips ||
                       resourceType == NaturalResourceType.RecycledPaper ||
                       resourceType == NaturalResourceType.Chemicals ||
                       resourceType == NaturalResourceType.Water ||
                       resourceType == NaturalResourceType.Energy;
            }
            return false;
        }

        private bool IsPackagingMaterial(object material)
        {
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Glass ||
                       resourceType == NaturalResourceType.Aluminium ||
                       resourceType == NaturalResourceType.Wood ||
                       resourceType == NaturalResourceType.Ink;
            }
            if (material is ProductType productType)
            {
                return productType == ProductType.CardboardSheets ||
                       productType == ProductType.Plastic;
            }
            return false;
        }

        // МЕТОДЫ ДЛЯ ПРОВЕРКИ МАТЕРИАЛОВ
        private bool IsFireEquipmentMaterial(object material)
        {
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Iron ||
                       resourceType == NaturalResourceType.Wood ||
                       resourceType == NaturalResourceType.Energy ||
                       resourceType == NaturalResourceType.Water;
            }
            if (material is ProductType productType)
            {
                return productType == ProductType.Electronics;
            }
            return false;
        }

        private bool IsRoboticsMaterial(object material)
        {
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Iron ||
                       resourceType == NaturalResourceType.Energy ||
                       resourceType == NaturalResourceType.Water;
            }
            if (material is ProductType productType)
            {
                return productType == ProductType.Electronics;
            }
            return false;
        }
    }
}