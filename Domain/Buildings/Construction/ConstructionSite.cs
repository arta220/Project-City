using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;
using System.Collections.Generic;

namespace Domain.Buildings.Construction
{
    /// <summary>
    /// Строительная площадка - временное здание, на котором происходит строительство 
    /// (заполнение тайла, чтобы предотвратить двойное построение)
    /// </summary>
    public class ConstructionSite : Building
    {
        /// <summary>
        /// Текущий проект строительства
        /// </summary>
        public ConstructionProject Project { get; set; }

        public Dictionary<Enum, int> MaterialsBank { get; } = new Dictionary<Enum, int>();

        /// <summary>
        /// Статус строительной площадки
        /// </summary>
        public ConstructionSiteStatus Status { get; set; }

        /// <summary>
        /// Флаг отмены строительства
        /// Отмена строительства приводит к остановке работ и демонтажу площадки
        /// </summary>
        public bool IsCancelled { get; set; }

        public ConstructionSite(Area area, ConstructionProject project)
            : base(floors: 0, maxOccupancy: project.MinWorkersRequired * 3, area)
        {
            Project = project;
            Status = ConstructionSiteStatus.Preparing;
            IsCancelled = false;

            // Инициализация вакансий для строителей
            Vacancies[CitizenProfession.ConstructionWorker] = project.MinWorkersRequired * 2;
            MaxAges[CitizenProfession.ConstructionWorker] = 60;
        }

        /// <summary>
        /// Добавляет материалы на строительную площадку
        /// </summary>
        /// <param name="materialType">тип материала</param>
        /// <param name="amount">количество материала</param>
        public void AddMaterials(Enum materialType, int amount)
        {
            if (!MaterialsBank.ContainsKey(materialType))
                MaterialsBank[materialType] = 0;
            
            MaterialsBank[materialType] += amount;
        }

        /// <summary>
        /// Использует материалы из банка (при строительстве)
        /// </summary>
        /// <param name="materialsToConsume">доступные материалы</param>
        /// <returns>True, если материалов достаточно (используются), иначе false (ни один материал не спиан)</returns>
        public bool ConsumeMaterials(Dictionary<Enum, int> materialsToConsume)
        {
            // Проверяем наличие всех материалов
            foreach (var material in materialsToConsume)
            {
                if (!MaterialsBank.TryGetValue(material.Key, out int amount) || 
                    amount < material.Value)
                {
                    return false;
                }
            }

            // Потребляем материалы
            foreach (var material in materialsToConsume)
            {
                MaterialsBank[material.Key] -= material.Value;
                if (MaterialsBank[material.Key] <= 0)
                    MaterialsBank.Remove(material.Key);
            }

            return true;
        }

        /// <summary>
        /// Проверяет, достаточно ли материалов для продолжения строительства
        /// </summary>
        public bool HasEnoughMaterials() => Project.HasAllMaterials(MaterialsBank);

        /// <summary>
        /// Получает процент готовности материалов
        /// </summary>
        public double GetMaterialsReadiness() => Project.GetMaterialsReadiness(MaterialsBank);

        /// <summary>
        /// Проверяет, достаточно ли рабочих для строительства
        /// </summary>
        public bool HasEnoughWorkers() => CurrentWorkers.Count >= Project.MinWorkersRequired;
    }
}

