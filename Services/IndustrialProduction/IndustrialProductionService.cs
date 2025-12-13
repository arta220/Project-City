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
        private int _totalCosmeticsProduction = 0;
        private int _totalAlcoholProduction = 0;
        private int _totalCardboardMaterialsUsed = 0;
        private int _totalPackagingMaterialsUsed = 0;
        private int _totalCosmeticsMaterialsUsed = 0;
        private int _totalAlcoholMaterialsUsed = 0;

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
            int cosmeticsProduction = 0;
            int alcoholProduction = 0;
            int cardboardMaterialsUsed = 0;
            int packagingMaterialsUsed = 0;
            int fireEquipmentProduction = 0;
            int roboticsProduction = 0;
            int fireEquipmentMaterialsUsed = 0;
            int roboticsMaterialsUsed = 0;
            int cosmeticsMaterialsUsed = 0;
            int alcoholMaterialsUsed = 0;

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
                // Подсчёт производства косметики
                var cosmeticsProducts = building.ProductsBank
                    .Where(kvp => IsCosmeticsProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);

                cosmeticsProduction += cosmeticsProducts;

                // Подсчёт производства алкоголя
                var alcoholProducts = building.ProductsBank
                    .Where(kvp => IsAlcoholProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);

                alcoholProduction += alcoholProducts;

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
                // Подсчёт использованных материалов для косметики
                var cosmeticsMaterials = building.MaterialsBank
                    .Where(kvp => IsCosmeticsMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);

                cosmeticsMaterialsUsed += cosmeticsMaterials;

                // Подсчёт использованных материалов для алкоголя
                var alcoholMaterials = building.MaterialsBank
                    .Where(kvp => IsAlcoholMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);

                alcoholMaterialsUsed += alcoholMaterials;
            }

            // Обновление общей статистики
            _totalCardboardProduction += cardboardProduction;
            _totalPackagingProduction += packagingProduction;
            _totalCosmeticsProduction += cosmeticsProduction;
            _totalAlcoholProduction += alcoholProduction;
            _totalCardboardMaterialsUsed += cardboardMaterialsUsed;
            _totalPackagingMaterialsUsed += packagingMaterialsUsed;
            _totalCosmeticsMaterialsUsed += cosmeticsMaterialsUsed;
            _totalAlcoholMaterialsUsed += alcoholMaterialsUsed;

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
                cosmeticsProduction,
                alcoholProduction,
                cardboardMaterialsUsed,
                packagingMaterialsUsed,
                fireEquipmentProduction,    // Новый параметр
                fireEquipmentMaterialsUsed, // Новый параметр
                roboticsProduction,         // Новый параметр
                roboticsMaterialsUsed       // Новый параметр
                cosmeticsMaterialsUsed,
                alcoholMaterialsUsed
            );

            // Добавление в историю
            _statistics.CardboardHistory.Add(dataPoint);
            _statistics.PackagingHistory.Add(dataPoint);
            _statistics.FireEquipmentHistory.Add(dataPoint);  // Новая история
            _statistics.RoboticsHistory.Add(dataPoint);       // Новая история
            _statistics.CosmeticsHistory.Add(dataPoint);
            _statistics.AlcoholHistory.Add(dataPoint);
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
        private bool IsCosmeticsProduct(object product)
        {
            if (product is ProductType productType)
            {
                return productType == ProductType.SkinCream ||
                       productType == ProductType.Shampoo ||
                       productType == ProductType.Perfume ||
                       productType == ProductType.Makeup ||
                       productType == ProductType.CosmeticBottle ||
                       productType == ProductType.HairCareProduct ||
                       productType == ProductType.Sunscreen ||
                       productType == ProductType.MakeupKit ||
                       productType == ProductType.HygieneProduct ||
                       productType == ProductType.ScentedCandle ||
                       productType == ProductType.CosmeticSet;
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
        private bool IsAlcoholProduct(object product)
        {
            if (product is ProductType productType)
            {
                return productType == ProductType.Beer ||
                       productType == ProductType.Vodka ||
                       productType == ProductType.Wine ||
                       productType == ProductType.Whiskey ||
                       productType == ProductType.Rum ||
                       productType == ProductType.Tequila ||
                       productType == ProductType.Gin ||
                       productType == ProductType.Brandy ||
                       productType == ProductType.Champagne ||
                       productType == ProductType.Liqueur;
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
        private bool IsCosmeticsMaterial(object material)
        {
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Chemicals ||
                       resourceType == NaturalResourceType.Water ||
                       resourceType == NaturalResourceType.Glass ||
                       resourceType == NaturalResourceType.Energy;
            }
            if (material is ProductType productType)
            {
                return productType == ProductType.Plastic;
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
        private bool IsAlcoholMaterial(object material)
        {
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Water ||
                       resourceType == NaturalResourceType.Energy;
            }
            if (material is ProductType productType)
            {
                return productType == ProductType.Plastic ||
                       productType == ProductType.GlassJar;
            }
            return false;
        }
    }
}