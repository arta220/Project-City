using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public class Hospital : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.Hospital;

        /// <summary>
        /// Эффективность лечения (1.0 = 100%)
        /// </summary>
        public float HealingEfficiency { get; private set; } = 1.0f;

        /// <summary>
        /// Количество здоровья, восстанавливаемое за тик
        /// </summary>
        public float HealingPerTick { get; private set; } = 0.5f;

        /// <summary>
        /// Радиус действия больницы (в клетках)
        /// </summary>
        public int HealingRadius { get; private set; } = 60;

        public override float MaxHealth => 150f; // Больницы прочнее обычных зданий

        public Hospital(Area area)
            : base(area, serviceTime: 25, maxQueue: 20, workerCount: 15)
        {
            // Увеличиваем эффективность в зависимости от размера
            HealingEfficiency = 1.0f + (area.Width * area.Height - 9) * 0.05f; // +5% за каждую дополнительную клетку
            HealingPerTick = 0.5f * HealingEfficiency;
        }

        /// <summary>
        /// Улучшает эффективность лечения
        /// </summary>
        public void UpgradeHealing()
        {
            HealingEfficiency += 0.15f; // +15% эффективности
            HealingPerTick = 0.5f * HealingEfficiency;
        }

        /// <summary>
        /// Увеличивает радиус лечения
        /// </summary>
        public void UpgradeRadius()
        {
            HealingRadius += 15; // +15 клеток радиуса
        }

        /// <summary>
        /// Лечит указанное здание
        /// </summary>
        public float HealBuilding(Building building)
        {
            float healAmount = HealingPerTick;
            building.Heal(healAmount);
            return healAmount;
        }
    }
}