using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Services.JewelryProduction
{
    public class JewelryProductionService : IJewelryProductionService
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly JewelryProductionStatistics _statistics = new();

        public List<int> GlobalProductionHistory { get; } = new();
        public List<decimal> GlobalRevenueHistory { get; } = new();

        private int _totalJewelryProduction = 0;
        private int _totalMaterialsUsed = 0;
        private decimal _totalRevenue = 0;

        public JewelryProductionService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        public void Update(SimulationTime time)
        {
            var jewelryBuildings = _buildingRegistry.GetBuildings<IndustrialBuilding>()
                .Where(b => b.Type == IndustrialBuildingType.JewelryFactory)
                .ToList();

            int totalProduction = 0;
            int materialsUsed = 0;
            
            // Статистика по категориям
            int ringsProduction = 0;
            int necklacesProduction = 0;
            int braceletsProduction = 0;
            int earringsProduction = 0;
            int pendantsProduction = 0;
            int premiumProduction = 0;
            int exclusiveProduction = 0;

            foreach (var building in jewelryBuildings)
            {
                // Запуск производства
                building.RunOnce();

                // Подсчёт производства по категориям
                foreach (var kvp in building.ProductsBank)
                {
                    if (!IsJewelryProduct(kvp.Key)) continue;
                    
                    var productType = (ProductType)kvp.Key;
                    int quantity = kvp.Value;
                    
                    totalProduction += quantity;
                    
                    // Категория: Кольца
                    if (IsRing(productType))
                    {
                        ringsProduction += quantity;
                    }
                    // Категория: Ожерелья
                    else if (IsNecklace(productType))
                    {
                        necklacesProduction += quantity;
                    }
                    // Категория: Браслеты
                    else if (IsBracelet(productType))
                    {
                        braceletsProduction += quantity;
                    }
                    // Категория: Серьги
                    else if (IsEarrings(productType))
                    {
                        earringsProduction += quantity;
                    }
                    // Категория: Кулоны
                    else if (IsPendant(productType))
                    {
                        pendantsProduction += quantity;
                    }
                    
                    // Премиум изделия
                    if (IsPremium(productType))
                    {
                        premiumProduction += quantity;
                    }
                    
                    // Эксклюзивные изделия
                    if (IsExclusive(productType))
                    {
                        exclusiveProduction += quantity;
                    }
                }

                // Подсчёт использованных материалов
                var materials = building.MaterialsBank
                    .Where(kvp => IsJewelryMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);

                materialsUsed += materials;
            }

            // Обновление статистики
            _totalJewelryProduction += totalProduction;
            _totalMaterialsUsed += materialsUsed;

            // Простой расчет дохода (можно улучшить)
            decimal revenue = totalProduction * 100; // Примерная стоимость за единицу
            _totalRevenue += revenue;

            // Добавление точки данных (всегда, даже если значения равны 0)
            var dataPoint = new ProductionDataPoint(
                time.TotalTicks,
                totalProduction,
                materialsUsed,
                ringsProduction,
                necklacesProduction,
                braceletsProduction,
                earringsProduction,
                pendantsProduction,
                premiumProduction,
                exclusiveProduction
            );

            _statistics.ProductionHistory.Add(dataPoint);

            // Обновление истории для графиков
            GlobalProductionHistory.Add(totalProduction);
            GlobalRevenueHistory.Add(revenue);
        }

        public JewelryProductionStatistics GetStatistics() => _statistics;

        private bool IsJewelryProduct(object product)
        {
            if (product is ProductType productType)
            {
                // Базовые ювелирные изделия
                return productType == ProductType.Ring ||
                       productType == ProductType.Necklace ||
                       productType == ProductType.Bracelet ||
                       productType == ProductType.Earrings ||
                       productType == ProductType.Pendant ||
                       // Премиум изделия
                       productType == ProductType.DiamondRing ||
                       productType == ProductType.RubyNecklace ||
                       productType == ProductType.EmeraldBracelet ||
                       productType == ProductType.PearlEarrings ||
                       productType == ProductType.SapphirePendant ||
                       // Эксклюзивные изделия
                       productType == ProductType.PlatinumRing ||
                       productType == ProductType.GoldNecklace ||
                       productType == ProductType.DiamondEarrings ||
                       productType == ProductType.MultiGemRing ||
                       // Драгоценные металлы и камни (как продукты)
                       productType == ProductType.Gold ||
                       productType == ProductType.Silver ||
                       productType == ProductType.Platinum ||
                       productType == ProductType.Diamond ||
                       productType == ProductType.Ruby ||
                       productType == ProductType.Emerald ||
                       productType == ProductType.Pearl ||
                       productType == ProductType.Sapphire;
            }
            return false;
        }

        private bool IsJewelryMaterial(object material)
        {
            // Материалы для ювелирного производства
            if (material is NaturalResourceType resourceType)
            {
                // Базовые ресурсы, которые могут использоваться в ювелирном производстве
                return resourceType == NaturalResourceType.Copper ||
                       resourceType == NaturalResourceType.Iron ||
                       resourceType == NaturalResourceType.Energy;
            }
            if (material is ProductType productType)
            {
                // Драгоценные металлы и камни как материалы
                return productType == ProductType.Gold ||
                       productType == ProductType.Silver ||
                       productType == ProductType.Platinum ||
                       productType == ProductType.Diamond ||
                       productType == ProductType.Ruby ||
                       productType == ProductType.Emerald ||
                       productType == ProductType.Pearl ||
                       productType == ProductType.Sapphire;
            }
            return false;
        }

        // Методы для определения категорий изделий
        private bool IsRing(ProductType productType)
        {
            return productType == ProductType.Ring ||
                   productType == ProductType.DiamondRing ||
                   productType == ProductType.PlatinumRing ||
                   productType == ProductType.MultiGemRing;
        }

        private bool IsNecklace(ProductType productType)
        {
            return productType == ProductType.Necklace ||
                   productType == ProductType.RubyNecklace ||
                   productType == ProductType.GoldNecklace;
        }

        private bool IsBracelet(ProductType productType)
        {
            return productType == ProductType.Bracelet ||
                   productType == ProductType.EmeraldBracelet;
        }

        private bool IsEarrings(ProductType productType)
        {
            return productType == ProductType.Earrings ||
                   productType == ProductType.PearlEarrings ||
                   productType == ProductType.DiamondEarrings;
        }

        private bool IsPendant(ProductType productType)
        {
            return productType == ProductType.Pendant ||
                   productType == ProductType.SapphirePendant;
        }

        private bool IsPremium(ProductType productType)
        {
            return productType == ProductType.DiamondRing ||
                   productType == ProductType.RubyNecklace ||
                   productType == ProductType.EmeraldBracelet ||
                   productType == ProductType.PearlEarrings ||
                   productType == ProductType.SapphirePendant ||
                   productType == ProductType.PlatinumRing ||
                   productType == ProductType.GoldNecklace ||
                   productType == ProductType.DiamondEarrings;
        }

        private bool IsExclusive(ProductType productType)
        {
            return productType == ProductType.MultiGemRing;
        }
    }
}

