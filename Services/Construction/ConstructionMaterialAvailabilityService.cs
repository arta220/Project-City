using Domain.Buildings;
using Domain.Buildings.Construction;
using Domain.Common.Enums;
using Domain.Factories;
using Services.BuildingRegistry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Construction
{
    /// <summary>
    /// Сервис для проверки доступности строительных материалов и получения информации о заводах
    /// </summary>
    public class ConstructionMaterialAvailabilityService : IConstructionMaterialAvailabilityService
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly Dictionary<Type, string> _factoryTypeToName = new();

        public ConstructionMaterialAvailabilityService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
            InitializeFactoryNames();
        }

        /// <summary>
        /// Проверяет доступность материалов для проекта строительства
        /// </summary>
        /// <param name="project">Проект строительства для проверки</param>
        /// <returns>Информация о доступности материалов</returns>
        public MaterialAvailabilityInfo CheckMaterialAvailability(ConstructionProject project)
        {
            var info = new MaterialAvailabilityInfo();

            if (project == null || project.RequiredMaterials == null)
                return info;

            foreach (var material in project.RequiredMaterials.Keys)
            {
                // Проверяем наличие заводов с этим материалом на карте
                var factoriesOnMap = FindFactoriesWithMaterial(material);
                
                // Если нет заводов на карте, проверяем, какие заводы могут производить этот материал
                if (factoriesOnMap.Count == 0)
                {
                    var availableFactories = GetFactoriesProducingMaterial(material);
                    if (availableFactories.Count > 0)
                    {
                        info.UnavailableMaterials[material] = availableFactories;
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// Получает список заводов, которые производят указанный материал
        /// </summary>
        /// <param name="materialType">Тип материала</param>
        /// <returns>Список названий заводов, производящих материал (из BuildingRegistry)</returns>
        public List<string> GetFactoriesProducingMaterial(Enum materialType)
        {
            var factories = new List<string>();

            // Определяем, какие заводы производят этот материал на основе их цехов
            // Используем информацию из BuildingRegistry для получения названий
            switch (materialType)
            {
                case ConstructionMaterialType.Cement:
                    factories.Add("Цементный завод");
                    break;
                case ConstructionMaterialType.Bricks:
                    factories.Add("Кирпичный завод");
                    break;
                case ConstructionMaterialType.Concrete:
                    factories.Add("Бетонный завод");
                    break;
                case ConstructionMaterialType.Rebar:
                    factories.Add("Завод ЖБИ");
                    break;
                case ProductType.Steel:
                    factories.Add("Завод");
                    break;
                case NaturalResourceType.Wood:
                    factories.Add("Склад");
                    break;
                case NaturalResourceType.Glass:
                    factories.Add("Завод упаковки");
                    break;
            }

            return factories;
        }

        /// <summary>
        /// Находит заводы на карте, которые имеют указанный материал
        /// </summary>
        /// <param name="materialType">Тип материала</param>
        /// <returns>Список названий заводов с материалом</returns>
        private List<string> FindFactoriesWithMaterial(Enum materialType)
        {
            var factories = new List<string>();
            var industrialBuildings = _buildingRegistry.GetBuildings<IndustrialBuilding>().ToList();

            foreach (var building in industrialBuildings)
            {
                bool hasMaterial = (building.ProductsBank.TryGetValue(materialType, out int productAmount) && productAmount > 0) ||
                                   (building.MaterialsBank.TryGetValue(materialType, out int materialAmount) && materialAmount > 0);

                if (hasMaterial)
                {
                    // Определяем название завода по его типу
                    var factoryName = GetFactoryName(building);
                    if (!string.IsNullOrEmpty(factoryName))
                    {
                        factories.Add(factoryName);
                    }
                }
            }

            return factories;
        }

        /// <summary>
        /// Получает название завода по его типу и характеристикам
        /// </summary>
        /// <param name="building">Промышленное здание</param>
        /// <returns>Название завода</returns>
        private string GetFactoryName(IndustrialBuilding building)
        {
            // Определяем название по типу и характеристикам здания
            // Это упрощенный подход - в реальности можно использовать BuildingRegistry для получения точных названий
            return building.Type switch
            {
                IndustrialBuildingType.Factory when building.Area.Width == 5 && building.Area.Height == 5 && building.Floors == 1 && building.MaxOccupancy == 50 
                    => "Завод",
                IndustrialBuildingType.Warehouse when building.Area.Width == 4 && building.Area.Height == 6 && building.Floors == 1 && building.MaxOccupancy == 10
                    => "Склад",
                IndustrialBuildingType.Factory when building.Area.Width == 5 && building.Area.Height == 5 && building.Floors == 2 && building.MaxOccupancy == 12
                    => "Завод картона",
                IndustrialBuildingType.Warehouse when building.Area.Width == 6 && building.Area.Height == 6 && building.Floors == 2 && building.MaxOccupancy == 15
                    => "Завод упаковки",
                IndustrialBuildingType.Warehouse when building.Area.Width == 5 && building.Area.Height == 5 && building.Floors == 2 && building.MaxOccupancy == 80
                    => "Фармацевтический завод",
                IndustrialBuildingType.Factory when building.Area.Width == 4 && building.Area.Height == 4 && building.Floors == 1 && building.MaxOccupancy == 60
                    => "Завод по переработке отходов",
                IndustrialBuildingType.Factory when building.Area.Width == 5 && building.Area.Height == 5 && building.Floors == 2 && building.MaxOccupancy == 40
                    => "Цементный завод",
                IndustrialBuildingType.Factory when building.Area.Width == 4 && building.Area.Height == 4 && building.Floors == 1 && building.MaxOccupancy == 30
                    => "Кирпичный завод",
                IndustrialBuildingType.Factory when building.Area.Width == 4 && building.Area.Height == 5 && building.Floors == 1 && building.MaxOccupancy == 25
                    => "Бетонный завод",
                IndustrialBuildingType.Factory when building.Area.Width == 6 && building.Area.Height == 6 && building.Floors == 2 && building.MaxOccupancy == 50
                    => "Завод ЖБИ",
                IndustrialBuildingType.Factory => "Завод",
                IndustrialBuildingType.Warehouse => "Склад",
                _ => "Промышленное здание"
            };
        }

        /// <summary>
        /// Инициализирует словарь соответствия типов фабрик и их названий
        /// </summary>
        private void InitializeFactoryNames()
        {
            _factoryTypeToName[typeof(CementFactory)] = "Цементный завод";
            _factoryTypeToName[typeof(BrickFactory)] = "Кирпичный завод";
            _factoryTypeToName[typeof(ConcreteFactory)] = "Бетонный завод";
            _factoryTypeToName[typeof(ReinforcedConcreteFactory)] = "Завод ЖБИ";
            _factoryTypeToName[typeof(FactoryBuildingFactory)] = "Завод";
            _factoryTypeToName[typeof(WarehouseFactory)] = "Склад";
            _factoryTypeToName[typeof(CardboardFactory)] = "Завод картона";
            _factoryTypeToName[typeof(PackagingFactory)] = "Завод упаковки";
            _factoryTypeToName[typeof(PharmaceuticalFactoryFactory)] = "Фармацевтический завод";
            _factoryTypeToName[typeof(RecyclingPlantFactoryFactory)] = "Завод по переработке отходов";
        }
    }
}

