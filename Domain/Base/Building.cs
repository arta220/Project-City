using Domain.Enums;
using Domain.Map;
using System.ComponentModel;

namespace Domain.Base
{
    public abstract class Building : MapObject, INotifyPropertyChanged
    {
        // Smirnov
        public event PropertyChangedEventHandler PropertyChanged;
    /// <summary>
    /// Базовый класс для всех типов зданий на карте.
    /// </summary>
    /// <remarks>
    /// Наследуется от <see cref="MapObject"/> и добавляет свойства этажности и максимальной вместимости.
    /// Может быть унаследован для жилых, коммерческих, промышленных зданий и других типов.
    /// </remarks>
    public abstract class Building : MapObject
    {
        /// <summary>
        /// Количество этажей в здании.
        /// </summary>
        /// <remarks>
        /// Используется для расчёта вместимости, визуализации и других механик, связанных с этажностью.
        /// </remarks>
        public int Floors { get; }

        /// <summary>
        /// Максимальное количество граждан или объектов, которые могут находиться в здании одновременно.
        /// </summary>
        /// <remarks>
        /// Можно использовать для определения плотности населения, рабочих мест, студентов и т.д.
        /// </remarks>
        public int MaxOccupancy { get; }
        public Dictionary<UtilityType, bool> UtilityStates { get; private set; } // Smirnov
        public bool HasBrokenUtilities => UtilityStates.Values.Contains(false); // Smirnov

        /// <summary>
        /// Создаёт здание с указанным количеством этажей, максимальной вместимостью и занимаемой площадью.
        /// </summary>
        /// <param name="floors">Количество этажей.</param>
        /// <param name="maxOccupancy">Максимальная вместимость здания.</param>
        /// <param name="area">Площадь, занимаемая зданием на карте.</param>
        /// <remarks>
        /// Для создания конкретного типа здания рекомендуется наследовать этот класс
        /// и задавать специфические свойства, такие как тип здания или функции (жилое, коммерческое и т.д.).
        /// </remarks>
        protected Building(int floors, int maxOccupancy, Area area) : base(area)
        {
            Floors = floors;
            MaxOccupancy = maxOccupancy;
            InitializeUtilities(); // Smirnov
        }

        // Smirnov
        private void InitializeUtilities()
        {
            UtilityStates = new Dictionary<UtilityType, bool>
            {
                [UtilityType.Electricity] = true,
                [UtilityType.Water] = true,
                [UtilityType.Gas] = true,
                [UtilityType.Waste] = true
            };
        }

        // Smirnov
        public void BreakUtility(UtilityType utilityType) 
        {
            UtilityStates[utilityType] = false;
            OnPropertyChanged(nameof(HasBrokenUtilities));
        }
        // Smirnov
        public void FixUtility(UtilityType utilityType)
        {
            UtilityStates[utilityType] = true;
            OnPropertyChanged(nameof(HasBrokenUtilities)); 
        }

        // Smirnov
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsUtilityWorking(UtilityType utilityType) => UtilityStates[utilityType]; // Smirnov
    }
}
