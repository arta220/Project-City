using Domain.Buildings.Construction;
using Domain.Factories;

namespace Services.Construction
{
    /// <summary>
    /// Интерфейс фабрики для создания проектов строительства из фабрик зданий
    /// </summary>
    public interface IConstructionProjectFactory
    {
        /// <summary>
        /// Создает проект строительства для указанной фабрики здания
        /// </summary>
        /// <param name="buildingFactory">Фабрика здания, для которого создается проект</param>
        /// <returns>Проект строительства или null, если для данного типа здания не определен проект</returns>
        ConstructionProject CreateProject(IMapObjectFactory buildingFactory);
    }
}

