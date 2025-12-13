using Domain.Buildings;
using Domain.Buildings.Disaster;
using Domain.Citizens;
using Domain.Citizens.States;
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
        
        // Поля для работы с вакансиями и работниками
        public Dictionary<CitizenProfession, int> Vacancies { get; protected set; } = new();
        public List<Citizen> CurrentWorkers { get; protected set; } = new();
        public Dictionary<CitizenProfession, int> MaxAges { get; protected set; } = new();

        /// <summary>
        /// Здоровье здания (от 0 до 100). При достижении 0 здание уничтожается.
        /// </summary>
        private float _health = 100f;
        public float Health
        {
            get => _health;
            set
            {
                _health = Math.Clamp(value, 0f, MaxHealth);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Максимальное здоровье здания.
        /// </summary>
        public virtual float MaxHealth => 100f; // Виртуальное свойство, можно переопределить

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
            CurrentWorkers = new List<Citizen>();
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
        /// Восстанавливает здание на указанное количество здоровья.
        /// </summary>
        /// <param name="healAmount">Количество восстанавливаемого здоровья</param>
        public void Heal(float healAmount)
        {
            Health += healAmount;
        }

        /// <summary>
        /// Восстанавливает здание полностью.
        /// </summary>
        public void RepairBuilding()
        {
            Health = MaxHealth;
        }


        // Методы для работы с вакансиями и работниками
        public virtual bool HasVacancy(CitizenProfession profession)
        {
            return Vacancies.ContainsKey(profession) && Vacancies[profession] > 0;
        }

        public virtual bool Hire(Citizen citizen)
        {
            if (!HasVacancy(citizen.Profession) || citizen.WorkPlace != null)
                return false;

            Vacancies[citizen.Profession]--;
            CurrentWorkers.Add(citizen);
            citizen.WorkPlace = this;

            return true;
        }

        public virtual bool Fire(Citizen citizen)
        {
            if (!CurrentWorkers.Contains(citizen) || citizen.WorkPlace != this)
                return false;

            CurrentWorkers.Remove(citizen);

            if (Vacancies.ContainsKey(citizen.Profession))
                Vacancies[citizen.Profession]++;

            citizen.WorkPlace = null;

            return true;
        }

        public virtual int GetWorkerCount() => CurrentWorkers.Count;

        public virtual bool IsWorker(Citizen citizen) => CurrentWorkers.Contains(citizen);
    }
}
