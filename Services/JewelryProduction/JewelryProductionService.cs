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

            int jewelryProduction = 0;
            int materialsUsed = 0;

            foreach (var building in jewelryBuildings)
            {
                // Запуск производства
                building.RunOnce();

                // Подсчёт производства ювелирных изделий
                var jewelryProducts = building.ProductsBank
                    .Where(kvp => IsJewelryProduct(kvp.Key))
                    .Sum(kvp => kvp.Value);

                jewelryProduction += jewelryProducts;

                // Подсчёт использованных материалов
                var materials = building.MaterialsBank
                    .Where(kvp => IsJewelryMaterial(kvp.Key))
                    .Sum(kvp => kvp.Value);

                materialsUsed += materials;
            }

            // Обновление статистики
            _totalJewelryProduction += jewelryProduction;
            _totalMaterialsUsed += materialsUsed;

            // Простой расчет дохода (можно улучшить)
            decimal revenue = jewelryProduction * 100; // Примерная стоимость за единицу
            _totalRevenue += revenue;

            // Добавление точки данных (всегда, даже если значения равны 0)
            var dataPoint = new ProductionDataPoint(
                time.TotalTicks,
                jewelryProduction,
                materialsUsed
            );

            _statistics.ProductionHistory.Add(dataPoint);

            // Обновление истории для графиков
            GlobalProductionHistory.Add(jewelryProduction);
            GlobalRevenueHistory.Add(revenue);
        }

        public JewelryProductionStatistics GetStatistics() => _statistics;

        private bool IsJewelryProduct(object product)
        {
            if (product is ProductType productType)
            {
                // Готовые ювелирные изделия
                return productType == ProductType.Ring ||
                       productType == ProductType.Necklace ||
                       productType == ProductType.Bracelet ||
                       productType == ProductType.Earrings ||
                       productType == ProductType.Pendant ||
                       // Драгоценные металлы и камни (как продукты)
                       productType == ProductType.Gold ||
                       productType == ProductType.Silver ||
                       productType == ProductType.Platinum ||
                       productType == ProductType.Diamond ||
                       productType == ProductType.Ruby ||
                       productType == ProductType.Emerald;
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
                       productType == ProductType.Emerald;
            }
            return false;
        }
    }
}

