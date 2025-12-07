using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Map;

namespace Domain.Common.Base
{
    /// <summary>
    /// Базовый класс для всех типов зданий на карте.
    /// </summary>
    public abstract class Building : MapObject
    {
        public int Floors { get; }
        public int MaxOccupancy { get; }
        // Поля для работы с вакансиями и работниками
        public Dictionary<CitizenProfession, int> Vacancies { get; protected set; } = new();
        public List<Citizen> CurrentWorkers { get; protected set; } = new();
        public Dictionary<CitizenProfession, int> MaxAges { get; protected set; } = new();

        protected Building(int floors, int maxOccupancy, Area area) : base(area)
        {
            Floors = floors;
            MaxOccupancy = maxOccupancy;
            CurrentWorkers = new List<Citizen>();
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
