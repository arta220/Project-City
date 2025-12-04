using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    /// <summary>
    /// Базовый класс для всех заводов.
    /// Нельзя создать напрямую, только через дочерние классы.
    /// </summary>
    public abstract class IndustrialBuilding : Building
    {
        /// <summary> Тип завода </summary>
        public IndustryType IndustryType { get; protected set; }

        /// <summary> Что потребляет завод </summary>
        public ResourseType InputResource { get; protected set; }

        /// <summary>Что производит завод </summary>
        public ResourseType OutputResource { get; protected set; }

        /// <summary> Число рабочих </summary>
        public int CurrentWorkers { get; protected set; }

        /// <summary> Сколько максимум рабочих может быть </summary>
        public int MaxWorkers => MaxOccupancy;

        /// <summary> Сколько продукции делает 1 рабочий </summary>
        public int ProductionPerWorker { get; protected set; }

        /// <summary> Сколько всего произвели </summary>
        public int TotalProduced { get; protected set; }

        /// <summary> Сколько денег заработали </summary>
        public int Profit { get; protected set; }

        /// <summary> Есть ли рабочие на заводе? </summary>
        public bool HasWorkers() => CurrentWorkers > 0;

        /// <summary>
        /// Создает завод
        /// </summary>
        /// <param name="floors">Этажи</param>
        /// <param name="maxOccupancy">Максимум людей</param>
        /// <param name="area">Площадь</param>
        protected IndustrialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
            // Начальные значения
            CurrentWorkers = 0;
            ProductionPerWorker = 0; // Зададут в наследниках
            TotalProduced = 0;
            Profit = 0;
        }
    }
}