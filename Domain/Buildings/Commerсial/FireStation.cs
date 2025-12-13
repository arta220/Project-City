using Domain.Common.Enums;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public class FireStation : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.FireStation;

        /// <summary>
        /// Эффективность тушения пожаров (1.0 = 100%)
        /// </summary>
        public float FireFightingEfficiency { get; private set; } = 1.0f;

        /// <summary>
        /// Радиус действия пожарной части (в клетках)
        /// </summary>
        public int FireFightingRadius { get; private set; } = 50;

        public FireStation(Area area)
            : base(area, serviceTime: 8, maxQueue: 10, workerCount: 8)
        {
            // Увеличиваем эффективность в зависимости от размера
            FireFightingEfficiency = 1.0f + (area.Width * area.Height - 4) * 0.1f; // +10% за каждую дополнительную клетку
        }

        /// <summary>
        /// Улучшает эффективность пожарной части
        /// </summary>
        public void UpgradeEfficiency()
        {
            FireFightingEfficiency += 0.2f; // +20% эффективности
        }

        /// <summary>
        /// Увеличивает радиус действия
        /// </summary>
        public void UpgradeRadius()
        {
            FireFightingRadius += 10; // +10 клеток радиуса
        }
    }
}