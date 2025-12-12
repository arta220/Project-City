using Domain.Buildings.Disaster;
using Domain.Map;
using System;

namespace Domain.Common.Base
{
    /// <summary>
    /// Базовый класс для всех типов зданий на карте.
    /// </summary>
    public abstract class Building : MapObject
    {
        public int Floors { get; }
        public int MaxOccupancy { get; }
        public DisasterManager Disasters { get; }

        /// <summary>
        /// Здоровье здания (от 0 до 100). При достижении 0 здание уничтожается.
        /// </summary>
        private float _health = 100f;
        public float Health
        {
            get => _health;
            set
            {
                _health = Math.Clamp(value, 0f, 100f);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Максимальное здоровье здания.
        /// </summary>
        public const float MaxHealth = 100f;

        /// <summary>
        /// Проверяет, разрушено ли здание (здоровье <= 0).
        /// </summary>
        public bool IsDestroyed => Health <= 0f;

        protected Building(int floors, int maxOccupancy, Area area) : base(area)
        {
            Floors = floors;
            MaxOccupancy = maxOccupancy;
            Disasters = new DisasterManager();
            Health = MaxHealth;
        }

        /// <summary>
        /// Наносит урон зданию.
        /// </summary>
        /// <param name="damage">Величина урона</param>
        public void TakeDamage(float damage)
        {
            Health -= damage;
        }

        /// <summary>
        /// Восстанавливает здание, возвращая здоровье на максимальное значение.
        /// </summary>
        public void RepairBuilding()
        {
            Health = MaxHealth;
        }
    }
}
