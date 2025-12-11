using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Common;
using System.Linq;
using System.Diagnostics;

namespace Services.IndustrialProduction
{
    public class IndustrialProductionService : IIndustrialProductionService, IUpdatable
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly IndustrialProductionStatistics _statistics = new();

        private int _productionTickCounter = 0;
        private const int PRODUCTION_INTERVAL = 15; // Производство каждые 15 тиков

        // Для отслеживания изменений между тиками
        private class BuildingStats
        {
            public int LastProductCount { get; set; }
            public int LastMaterialCount { get; set; }
        }

        private readonly Dictionary<IndustrialBuilding, BuildingStats> _lastStats = new();

        public IndustrialProductionService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        public void Update(SimulationTime time)
        {
            _productionTickCounter++;

            if (_productionTickCounter < PRODUCTION_INTERVAL)
                return;

            _productionTickCounter = 0;

            var industrialBuildings = _buildingRegistry.GetBuildings<IndustrialBuilding>().ToList();

            Debug.WriteLine($"=== IndustrialProductionService.Update() ===");
            Debug.WriteLine($"Тик: {time.TotalTicks}, Заводов найдено: {industrialBuildings.Count}");

            // Счетчики для ВСЕХ типов заводов
            int cardboardProduction = 0;
            int packagingProduction = 0;
            int resourceExtractionProduction = 0;
            int woodProcessingProduction = 0;
            int recyclingProcessingProduction = 0;

            int cardboardMaterialsUsed = 0;
            int packagingMaterialsUsed = 0;
            int resourceExtractionMaterialsUsed = 0;
            int woodProcessingMaterialsUsed = 0;
            int recyclingProcessingMaterialsUsed = 0;

            foreach (var building in industrialBuildings)
            {
                Debug.WriteLine($"\nОбработка завода: {building.GetType().Name}");
                Debug.WriteLine($"Рабочих: {building.GetWorkerCount()}");
                Debug.WriteLine($"Продукты: {string.Join(", ", building.ProductsBank.Keys)}");
                Debug.WriteLine($"Материалы: {string.Join(", ", building.MaterialsBank.Keys)}");

                // Запускаем производство если есть рабочие
                if (building.GetWorkerCount() > 0)
                {
                    building.RunOnce();
                }

                // Инициализируем статистику для нового здания
                if (!_lastStats.ContainsKey(building))
                {
                    Debug.WriteLine($"Новый завод, инициализируем статистику");
                    _lastStats[building] = new BuildingStats
                    {
                        LastProductCount = building.ProductsBank.Values.Sum(),
                        LastMaterialCount = building.MaterialsBank.Values.Sum()
                    };
                    continue; // Пропускаем первый тик для сбора базовых данных
                }

                var currentProducts = building.ProductsBank.Values.Sum();
                var currentMaterials = building.MaterialsBank.Values.Sum();
                var lastStats = _lastStats[building];

                // Вычисляем изменения с последнего тика
                var producedSinceLastTick = currentProducts - lastStats.LastProductCount;
                var usedSinceLastTick = currentMaterials - lastStats.LastMaterialCount;

                Debug.WriteLine($"Изменения: Продукты +{producedSinceLastTick}, Материалы -{usedSinceLastTick}");

                // Обновляем последние значения
                lastStats.LastProductCount = currentProducts;
                lastStats.LastMaterialCount = currentMaterials;

                // Определяем тип завода по ПРОДУКЦИИ, которую он производит
                bool hasCardboardProducts = building.ProductsBank.Keys.Any(IsCardboardProduct);
                bool hasPackagingProducts = building.ProductsBank.Keys.Any(IsPackagingProduct);
                bool hasResourceExtractionProducts = building.ProductsBank.Keys.Any(IsResourceExtractionProduct);
                bool hasWoodProcessingProducts = building.ProductsBank.Keys.Any(IsWoodProcessingProduct);
                bool hasRecyclingProducts = building.ProductsBank.Keys.Any(IsRecyclingProduct);

                // Проверка по материалам
                bool usesCardboardMaterials = building.MaterialsBank.Keys.Any(IsCardboardMaterial);
                bool usesPackagingMaterials = building.MaterialsBank.Keys.Any(IsPackagingMaterial);

                Debug.WriteLine($"Определение типа:");
                Debug.WriteLine($"  Картонные продукты: {hasCardboardProducts}");
                Debug.WriteLine($"  Упаковочные продукты: {hasPackagingProducts}");
                Debug.WriteLine($"  Ресурсные продукты: {hasResourceExtractionProducts}");
                Debug.WriteLine($"  Деревообработка: {hasWoodProcessingProducts}");
                Debug.WriteLine($"  Переработка: {hasRecyclingProducts}");
                Debug.WriteLine($"  Картонные материалы: {usesCardboardMaterials}");
                Debug.WriteLine($"  Упаковочные материалы: {usesPackagingMaterials}");

                // НЕЗАВИСИМЫЕ ПРОВЕРКИ - завод может быть нескольких типов!

                // КАРТОННЫЙ ЗАВОД
                if (hasCardboardProducts || usesCardboardMaterials)
                {
                    cardboardProduction += producedSinceLastTick;
                    cardboardMaterialsUsed += usedSinceLastTick;
                    Debug.WriteLine($"  => Отнесен к КАРТОННЫМ заводам");
                }

                // УПАКОВОЧНЫЙ ЗАВОД
                if (hasPackagingProducts || usesPackagingMaterials)
                {
                    packagingProduction += producedSinceLastTick;
                    packagingMaterialsUsed += usedSinceLastTick;
                    Debug.WriteLine($"  => Отнесен к УПАКОВОЧНЫМ заводам");
                }

                // РЕСУРСО-ДОБЫВАЮЩИЙ ЗАВОД
                if (hasResourceExtractionProducts)
                {
                    resourceExtractionProduction += producedSinceLastTick;
                    resourceExtractionMaterialsUsed += usedSinceLastTick;
                    Debug.WriteLine($"  => Отнесен к РЕСУРСО-ДОБЫВАЮЩИМ заводам");
                }

                // ДЕРЕВООБРАБАТЫВАЮЩИЙ ЗАВОД
                if (hasWoodProcessingProducts)
                {
                    woodProcessingProduction += producedSinceLastTick;
                    woodProcessingMaterialsUsed += usedSinceLastTick;
                    Debug.WriteLine($"  => Отнесен к ДЕРЕВООБРАБАТЫВАЮЩИМ заводам");
                }

                // ПЕРЕРАБАТЫВАЮЩИЙ ЗАВОД
                if (hasRecyclingProducts)
                {
                    recyclingProcessingProduction += producedSinceLastTick;
                    recyclingProcessingMaterialsUsed += usedSinceLastTick;
                    Debug.WriteLine($"  => Отнесен к ПЕРЕРАБАТЫВАЮЩИМ заводам");
                }
            }

            // Создаем точку данных
            var dataPoint = new ProductionDataPoint
            {
                Tick = time.TotalTicks,

                // Производство (ИЗМЕНЕНИЕ с последнего тика)
                CardboardProduction = cardboardProduction,
                PackagingProduction = packagingProduction,
                ResourceExtractionProduction = resourceExtractionProduction,
                WoodProcessingProduction = woodProcessingProduction,
                RecyclingProcessingProduction = recyclingProcessingProduction,

                // Использованные материалы (ИЗМЕНЕНИЕ с последнего тика)
                CardboardMaterialsUsed = cardboardMaterialsUsed,
                PackagingMaterialsUsed = packagingMaterialsUsed,
                ResourceExtractionMaterialsUsed = resourceExtractionMaterialsUsed,
                WoodProcessingMaterialsUsed = woodProcessingMaterialsUsed,
                RecyclingProcessingMaterialsUsed = recyclingProcessingMaterialsUsed
            };

            // Добавляем в ВСЕ истории
            _statistics.CardboardHistory.Add(dataPoint);
            _statistics.PackagingHistory.Add(dataPoint);
            _statistics.ResourceExtractionHistory.Add(dataPoint);
            _statistics.WoodProcessingHistory.Add(dataPoint);
            _statistics.RecyclingProcessingHistory.Add(dataPoint);

            Debug.WriteLine($"\nИтоги за тик {time.TotalTicks}:");
            Debug.WriteLine($"Картон: +{cardboardProduction} (мат: {cardboardMaterialsUsed})");
            Debug.WriteLine($"Упаковка: +{packagingProduction} (мат: {packagingMaterialsUsed})");
            Debug.WriteLine($"Ресурсы: +{resourceExtractionProduction} (мат: {resourceExtractionMaterialsUsed})");
            Debug.WriteLine($"Деревообработка: +{woodProcessingProduction} (мат: {woodProcessingMaterialsUsed})");
            Debug.WriteLine($"Переработка: +{recyclingProcessingProduction} (мат: {recyclingProcessingMaterialsUsed})");
        }

        public IndustrialProductionStatistics GetStatistics() => _statistics;

        // МЕТОДЫ ДЛЯ ОПРЕДЕЛЕНИЯ ТИПОВ ПРОДУКЦИИ

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

        // НОВЫЕ МЕТОДЫ ДЛЯ ДРУГИХ ТИПОВ ЗАВОДОВ - УБРАЛ НЕСУЩЕСТВУЮЩИЕ ТИПЫ
        private bool IsResourceExtractionProduct(object product)
        {
            // ResourceExtractionFactory производит NaturalResourceType
            if (product is NaturalResourceType resourceType)
            {
                return resourceType == NaturalResourceType.Iron ||
                       resourceType == NaturalResourceType.Wood ||
                       resourceType == NaturalResourceType.Coal ||
                       resourceType == NaturalResourceType.Oil ||
                       resourceType == NaturalResourceType.Glass ||
                       resourceType == NaturalResourceType.Aluminium;
                // Убрал NaturalResourceType.Stone - его нет
            }
            return false;
        }

        private bool IsWoodProcessingProduct(object product)
        {
            // WoodProcessingFactory производит ProductType
            if (product is ProductType productType)
            {
                return productType == ProductType.Lumber ||
                       productType == ProductType.Furniture ||
                       productType == ProductType.Paper ||
                       productType == ProductType.WoodenCrate;
                // Убрал ProductType.Plywood - его нет
            }
            return false;
        }

        private bool IsRecyclingProduct(object product)
        {
            // RecyclingFactory производит ProductType
            if (product is ProductType productType)
            {
                return productType == ProductType.Steel ||
                       productType == ProductType.Plastic ||
                       productType == ProductType.Fuel ||
                       productType == ProductType.PlasticBottle;
                // Убрал ProductType.GlassContainer и ProductType.MetalParts - их нет
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
    }
}