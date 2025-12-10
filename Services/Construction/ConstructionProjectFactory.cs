using Domain.Buildings.Construction;
using Domain.Common.Enums;
using Domain.Factories;
using System.Collections.Generic;

namespace Services.Construction
{
    /// <summary>
    /// Фабрика для создания проектов строительства из фабрик зданий
    /// Определяет требования к материалам и параметры строительства для каждого типа здания
    /// </summary>
    public class ConstructionProjectFactory : IConstructionProjectFactory
    {
        /// <summary>
        /// Создает проект строительства для указанной фабрики здания
        /// </summary>
        /// <param name="buildingFactory">Фабрика здания, для которого создается проект</param>
        /// <returns>Проект строительства или null, если для данного типа здания не определен проект</returns>
        public ConstructionProject CreateProject(IMapObjectFactory buildingFactory)
        {
            // Заводы (IndustrialBuilding) размещаются напрямую без строительства
            // Проверяем, является ли фабрика заводом
            if (IsIndustrialBuildingFactory(buildingFactory))
            {
                return null; // Для заводов не создаем проекты строительства
            }

            return buildingFactory switch
            {
                SmallHouseFactory => CreateSmallHouseProject(),
                ApartmentFactory => CreateApartmentProject(),
                _ => null // Для других типов зданий строительство не требуется
            };
        }

        /// <summary>
        /// Проверяет, является ли фабрика фабрикой промышленного здания (завода)
        /// </summary>
        /// <param name="factory">Фабрика для проверки</param>
        /// <returns>True, если это фабрика завода, иначе false</returns>
        private bool IsIndustrialBuildingFactory(IMapObjectFactory factory)
        {
            return factory is FactoryBuildingFactory ||
                   factory is WarehouseFactory ||
                   factory is CardboardFactory ||
                   factory is PackagingFactory ||
                   factory is PharmaceuticalFactoryFactory ||
                   factory is RecyclingPlantFactoryFactory ||
                   factory is CementFactory ||
                   factory is BrickFactory ||
                   factory is ConcreteFactory ||
                   factory is ReinforcedConcreteFactory;
        }

        /// <summary>
        /// Создает проект строительства для маленького дома
        /// </summary>
        /// <returns>Проект строительства маленького дома</returns>
        private ConstructionProject CreateSmallHouseProject()
        {
            return new ConstructionProject(
                targetBuildingFactory: new SmallHouseFactory(),
                requiredMaterials: new Dictionary<Enum, int>
                {
                    { ConstructionMaterialType.Bricks, 100 },
                    { ConstructionMaterialType.Cement, 50 },
                    { NaturalResourceType.Wood, 30 },
                    { NaturalResourceType.Glass, 10 }
                },
                constructionSpeed: 1,
                minWorkersRequired: 2,
                totalTicksRequired: 200
            );
        }

        /// <summary>
        /// Создает проект строительства для многоквартирного дома
        /// </summary>
        /// <returns>Проект строительства многоквартирного дома</returns>
        private ConstructionProject CreateApartmentProject()
        {
            return new ConstructionProject(
                targetBuildingFactory: new ApartmentFactory(),
                requiredMaterials: new Dictionary<Enum, int>
                {
                    { ConstructionMaterialType.Concrete, 500 },
                    { ConstructionMaterialType.Rebar, 200 },
                    { ConstructionMaterialType.Bricks, 300 },
                    { NaturalResourceType.Glass, 50 },
                    { ProductType.Steel, 100 }
                },
                constructionSpeed: 1,
                minWorkersRequired: 5,
                totalTicksRequired: 500
            );
        }

    }
}

