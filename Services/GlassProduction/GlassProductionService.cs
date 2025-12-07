using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Services.GlassProduction
{
    public class GlassProductionService : IGlassProductionService
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly GlassProductionStatistics _statistics = new();

        public List<int> GlobalProductionHistory { get; } = new();
        public List<decimal> GlobalRevenueHistory { get; } = new();

        private int _totalGlassProduction = 0;
        private int _totalMaterialsUsed = 0;
        private decimal _totalRevenue = 0;

        public GlassProductionService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        public void Update(SimulationTime time)
        {
            var glassBuildings = _buildingRegistry.GetBuildings<IndustrialBuilding>()
                .Where(b => b.Type == IndustrialBuildingType.GlassFactory)
                .ToList();

            int totalProduction = 0;
            int materialsUsed = 0;
            
            // Статистика по категориям
            int bottlesProduction = 0;
            int vasesProduction = 0;
            int windowsProduction = 0;
            int mirrorsProduction = 0;
            int tablewareProduction = 0;
            int premiumProduction = 0;
            int exclusiveProduction = 0;

            foreach (var building in glassBuildings)
            {
                // Запуск производства
                building.RunOnce();

                // Подсчёт производства по категориям
                foreach (var kvp in building.ProductsBank)
                {
                    if (!IsGlassProduct(kvp.Key)) continue;
                    
                    var productType = (ProductType)kvp.Key;
                    int quantity = kvp.Value;
                    
                    totalProduction += quantity;
                    
                    // Категория: Бутылки
                    if (IsBottle(productType))
                    {
                        bottlesProduction += quantity;
                    }
                    // Категория: Вазы
                    else if (IsVase(productType))
                    {
                        vasesProduction += quantity;
                    }
                    // Категория: Окна
                    else if (IsWindow(productType))
                    {
                        windowsProduction += quantity;
                    }
                    // Категория: Зеркала
                    else if (IsMirror(productType))
                    {
                        mirrorsProduction += quantity;
                    }
                    // Категория: Посуда
                    else if (IsTableware(productType))
                    {
                        tablewareProduction += quantity;
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
                    .Where(kvp => IsGlassMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);

                materialsUsed += materials;
            }

            // Обновление статистики
            _totalGlassProduction += totalProduction;
            _totalMaterialsUsed += materialsUsed;

            // Простой расчет дохода
            decimal revenue = totalProduction * 80; // Стоимость за единицу
            _totalRevenue += revenue;

            // Добавление точки данных (всегда, даже если значения равны 0)
            var dataPoint = new ProductionDataPoint(
                time.TotalTicks,
                totalProduction,
                materialsUsed,
                bottlesProduction,
                vasesProduction,
                windowsProduction,
                mirrorsProduction,
                tablewareProduction,
                premiumProduction,
                exclusiveProduction
            );

            _statistics.ProductionHistory.Add(dataPoint);

            // Обновление истории для графиков
            GlobalProductionHistory.Add(totalProduction);
            GlobalRevenueHistory.Add(revenue);
        }

        public GlassProductionStatistics GetStatistics() => _statistics;

        private bool IsGlassProduct(object product)
        {
            if (product is ProductType productType)
            {
                // Стекольные материалы
                return productType == ProductType.RawGlass ||
                       productType == ProductType.ColoredGlass ||
                       productType == ProductType.TemperedGlass ||
                       productType == ProductType.CrystalGlass ||
                       // Базовые стекольные изделия
                       productType == ProductType.GlassBottle ||
                       productType == ProductType.GlassVase ||
                       productType == ProductType.GlassWindow ||
                       productType == ProductType.GlassMirror ||
                       productType == ProductType.GlassTableware ||
                       // Премиум изделия
                       productType == ProductType.CrystalVase ||
                       productType == ProductType.StainedGlass ||
                       productType == ProductType.GlassSculpture ||
                       productType == ProductType.DecorativeGlass ||
                       // Эксклюзивные изделия
                       productType == ProductType.ArtGlass ||
                       productType == ProductType.LuxuryGlassware;
            }
            return false;
        }

        private bool IsGlassMaterial(object material)
        {
            // Материалы для стекольного производства
            if (material is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Glass ||
                       resourceType == NaturalResourceType.Energy ||
                       resourceType == NaturalResourceType.Chemicals ||
                       resourceType == NaturalResourceType.Water;
            }
            if (material is ProductType productType)
            {
                // Промежуточные стекольные материалы
                return productType == ProductType.RawGlass ||
                       productType == ProductType.ColoredGlass ||
                       productType == ProductType.TemperedGlass ||
                       productType == ProductType.CrystalGlass;
            }
            return false;
        }

        // Методы для определения категорий изделий
        private bool IsBottle(ProductType productType)
        {
            return productType == ProductType.GlassBottle;
        }

        private bool IsVase(ProductType productType)
        {
            return productType == ProductType.GlassVase ||
                   productType == ProductType.CrystalVase;
        }

        private bool IsWindow(ProductType productType)
        {
            return productType == ProductType.GlassWindow ||
                   productType == ProductType.StainedGlass;
        }

        private bool IsMirror(ProductType productType)
        {
            return productType == ProductType.GlassMirror;
        }

        private bool IsTableware(ProductType productType)
        {
            return productType == ProductType.GlassTableware ||
                   productType == ProductType.LuxuryGlassware;
        }

        private bool IsPremium(ProductType productType)
        {
            return productType == ProductType.CrystalVase ||
                   productType == ProductType.StainedGlass ||
                   productType == ProductType.GlassSculpture ||
                   productType == ProductType.DecorativeGlass;
        }

        private bool IsExclusive(ProductType productType)
        {
            return productType == ProductType.ArtGlass ||
                   productType == ProductType.LuxuryGlassware;
        }
    }
}
