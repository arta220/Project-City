using Domain.Buildings.Construction;
using Domain.Factories;
using System.Collections.Generic;

namespace Services.Construction
{
    /// <summary>
    /// Интерфейс сервиса для проверки доступности материалов для строительства
    /// </summary>
    public interface IConstructionMaterialAvailabilityService
    {
        /// <summary>
        /// Проверяет доступность материалов для проекта строительства
        /// </summary>
        /// <param name="project">Проект строительства для проверки</param>
        /// <returns>Информация о доступности материалов</returns>
        MaterialAvailabilityInfo CheckMaterialAvailability(ConstructionProject project);

        /// <summary>
        /// Получает список заводов, которые производят указанный материал
        /// </summary>
        /// <param name="materialType">Тип материала</param>
        /// <returns>Список названий заводов, производящих материал</returns>
        List<string> GetFactoriesProducingMaterial(System.Enum materialType);
    }

    /// <summary>
    /// Информация о доступности материалов для строительства
    /// </summary>
    public class MaterialAvailabilityInfo
    {
        /// <summary>
        /// Словарь материалов, которые недоступны (материал -> список заводов, которые его производят)
        /// </summary>
        public Dictionary<System.Enum, List<string>> UnavailableMaterials { get; } = new();

        /// <summary>
        /// Все ли материалы доступны
        /// </summary>
        public bool AllMaterialsAvailable => UnavailableMaterials.Count == 0;
    }
}

