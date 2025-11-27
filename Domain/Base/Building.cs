using Domain.Enums;
using Domain.Map;
using System.ComponentModel;

namespace Domain.Base
{
    public abstract class Building : MapObject, INotifyPropertyChanged
    {
        // Smirnov
        public event PropertyChangedEventHandler PropertyChanged;
        public int Floors { get; }
        public int MaxOccupancy { get; }
        public Dictionary<UtilityType, bool> UtilityStates { get; private set; } // Smirnov
        public bool HasBrokenUtilities => UtilityStates.Values.Contains(false); // Smirnov

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
