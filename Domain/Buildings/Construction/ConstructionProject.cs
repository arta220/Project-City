using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Buildings.Construction
{
    /// <summary>
    /// Представляет проект строительства здания
    /// </summary>
    public class ConstructionProject
    {
        /// <summary>
        /// Тип здания, которое будет построено
        /// </summary>
        public IMapObjectFactory TargetBuildingFactory { get; }

        /// <summary>
        /// Требуемые материалы для строительства
        /// </summary>
        public Dictionary<Enum, int> RequiredMaterials { get; }

        /// <summary>
        /// Текущий прогресс строительства (0-100)
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Скорость строительства (прогресс за тик)
        /// </summary>
        public int ConstructionSpeed { get; }

        /// <summary>
        /// Минимальное количество строителей для работы
        /// </summary>
        public int MinWorkersRequired { get; }

        /// <summary>
        /// Общее количество тиков, необходимое для строительства
        /// </summary>
        public int TotalTicksRequired { get; }

        /// <summary>
        /// Текущее количество тиков строительства
        /// </summary>
        public int CurrentTicks { get; set; }

        public ConstructionProject(
            IMapObjectFactory targetBuildingFactory,
            Dictionary<Enum, int> requiredMaterials,
            int constructionSpeed = 1,
            int minWorkersRequired = 1,
            int totalTicksRequired = 100)
        {
            TargetBuildingFactory = targetBuildingFactory;
            RequiredMaterials = requiredMaterials ?? new Dictionary<Enum, int>();
            ConstructionSpeed = constructionSpeed;
            MinWorkersRequired = minWorkersRequired;
            TotalTicksRequired = totalTicksRequired;
            Progress = 0;
            CurrentTicks = 0;
        }

        /// <summary>
        /// Проверяет, есть ли все необходимые материалы
        /// </summary>
        /// <param name="availableMaterials">Доступные материалы</param>
        /// <returns>True, если материалов достаточно, иначе false</returns>
        public bool HasAllMaterials(Dictionary<Enum, int> availableMaterials)
        {
            foreach (var requirement in RequiredMaterials)
            {
                if (!availableMaterials.TryGetValue(requirement.Key, out int amount) || 
                    amount < requirement.Value)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Вычисляет процент готовности материалов (для прогресс бара)
        /// </summary>
        /// <param name="availableMaterials">доступные материалы</param>
        /// <returns>Коэффициент готовности материалов (дробное)</returns>
        public double GetMaterialsReadiness(Dictionary<Enum, int> availableMaterials)
        {
            if (RequiredMaterials.Count == 0) return 1.0;

            int totalRequired = RequiredMaterials.Values.Sum();
            int totalAvailable = 0;

            foreach (var requirement in RequiredMaterials)
            {
                if (availableMaterials.TryGetValue(requirement.Key, out int amount))
                {
                    totalAvailable += Math.Min(amount, requirement.Value);
                }
            }

            return totalRequired > 0 ? (double)totalAvailable / totalRequired : 0.0;
        }
    }
}

