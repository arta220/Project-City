using System.ComponentModel;
using Domain.Base;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public class ResidentialBuilding : Building
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Dictionary<UtilityType, bool> UtilityStates { get; private set; }
        public bool HasBrokenUtilities => UtilityStates.Values.Contains(false);

        public ResidentialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
            InitializeUtilities();
        }

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

        public void BreakUtility(UtilityType utilityType)
        {
            UtilityStates[utilityType] = false;
            OnPropertyChanged(nameof(HasBrokenUtilities));
        }

        public void FixUtility(UtilityType utilityType)
        {
            UtilityStates[utilityType] = true;
            OnPropertyChanged(nameof(HasBrokenUtilities));
        }

        public bool IsUtilityWorking(UtilityType utilityType) => UtilityStates[utilityType];

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}