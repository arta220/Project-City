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
         // Статистика сельского хозяйства
        private int _totalAgricultureProduction = 0;
        private int _totalAgricultureMaterialsUsed = 0;
        // Статистика рыбодобывающей отрасли
        private int _totalFishingProduction = 0;
        private int _totalFishingMaterialsUsed = 0;

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
             // Статистика сельского хозяйства
            int agricultureProduction = 0;
            int agricultureMaterialsUsed = 0;
            // Статистика рыбодобывающей отрасли
            int fishingProduction = 0;
            int fishingMaterialsUsed = 0;

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

                 // Подсчёт производства сельскохозяйственной продукции
                var agricultureProducts = building.ProductsBank
                    .Where(kvp => IsAgricultureProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);

                agricultureProduction += agricultureProducts;

                // Подсчёт производства рыбодобывающей отрасли
                var fishingProducts = building.ProductsBank
                    .Where(kvp => IsFishingProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);

                fishingProduction += fishingProducts;

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

                // Подсчёт использованных материалов для сельского хозяйства
                var agricultureMaterials = building.MaterialsBank
                    .Where(kvp => IsAgricultureMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);

                agricultureMaterialsUsed += agricultureMaterials;

                // Подсчёт использованных материалов для рыбодобывающей отрасли
                var fishingMaterials = building.MaterialsBank
                    .Where(kvp => IsFishingMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);

                fishingMaterialsUsed += fishingMaterials;
            }

            // Обновление статистики
            _totalCardboardProduction += cardboardProduction;
            _totalPackagingProduction += packagingProduction;
            _totalCardboardMaterialsUsed += cardboardMaterialsUsed;
            _totalPackagingMaterialsUsed += packagingMaterialsUsed;
            _totalAgricultureProduction += agricultureProduction;
            _totalAgricultureMaterialsUsed += agricultureMaterialsUsed;
            _totalFishingProduction += fishingProduction;
            _totalFishingMaterialsUsed += fishingMaterialsUsed;

            // Добавление точки данных
            var dataPoint = new ProductionDataPoint(
                time.TotalTicks,
                cardboardProduction,
                packagingProduction,
                cardboardMaterialsUsed,
                packagingMaterialsUsed
            );

            _statistics.CardboardHistory.Add(dataPoint);
            _statistics.PackagingHistory.Add(dataPoint);
        }
        
          // Добавление точки данных для сельского хозяйства
            var agricultureDataPoint = new ProductionDataPoint(
                time.TotalTicks,
                agricultureProduction,
                agricultureMaterialsUsed
            );

            _statistics.AgricultureHistory.Add(agricultureDataPoint);

            // Добавление точки данных для рыбодобывающей отрасли
            var fishingDataPoint = new ProductionDataPoint(
                time.TotalTicks,
                fishingProduction,
                fishingMaterialsUsed,
                true
            );

            _statistics.FishingHistory.Add(fishingDataPoint);

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
        
        /// <summary>
        /// Проверяет, является ли продукт сельскохозяйственной продукцией
        /// </summary>
        private bool IsAgricultureProduct(object product)
        {
            if (product is ProductType productType)
            {
                return productType == ProductType.Crops ||
                       productType == ProductType.Vegetables ||
                       productType == ProductType.Fruits ||
                       productType == ProductType.Grains ||
                       productType == ProductType.DairyProducts ||
                       productType == ProductType.Meat ||
                       productType == ProductType.Eggs ||
                       productType == ProductType.ProcessedFood;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, является ли продукт продукцией рыбодобывающей отрасли
        /// </summary>
        private bool IsFishingProduct(object product)
        {
            if (product is ProductType productType)
            {
                return productType == ProductType.ProcessedFish ||
                       productType == ProductType.CannedFish ||
                       productType == ProductType.FrozenFish ||
                       productType == ProductType.FishProducts ||
                       productType == ProductType.Fishmeal;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, является ли материал ресурсом для сельского хозяйства
        /// </summary>
        private bool IsAgricultureMaterial(object material)
        {
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Seeds ||
                       resourceType == NaturalResourceType.Fertilizer ||
                       resourceType == NaturalResourceType.Feed ||
                       resourceType == NaturalResourceType.Livestock ||
                       resourceType == NaturalResourceType.Chemicals ||
                       resourceType == NaturalResourceType.Water ||
                       resourceType == NaturalResourceType.Energy;
            }
            if (material is ProductType productType)
            {
                return productType == ProductType.Crops;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, является ли материал ресурсом для рыбодобывающей отрасли
        /// </summary>
        private bool IsFishingMaterial(object material)
        {
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Fish ||
                       resourceType == NaturalResourceType.Ice ||
                       resourceType == NaturalResourceType.FuelForFleet ||
                       resourceType == NaturalResourceType.Energy ||
                       resourceType == NaturalResourceType.Water;
            }
            if (material is ProductType productType)
            {
                return productType == ProductType.ProcessedFish;
            }
            return false;
        }
    }
}

