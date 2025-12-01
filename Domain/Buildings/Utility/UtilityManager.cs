using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Enums;

namespace Domain.Buildings.Utility
{
    /// <summary>
    /// Менеджер коммунальных систем для жилых зданий.
    /// </summary>
    public class UtilityManager : ObservableObject
    {
        private readonly Dictionary<UtilityType, bool> _states;

        public UtilityManager()
        {
            _states = new Dictionary<UtilityType, bool>
            {
                [UtilityType.Electricity] = true,
                [UtilityType.Water] = true,
                [UtilityType.Gas] = true,
                [UtilityType.Waste] = true
            };
        }

        public bool HasBrokenUtilities => _states.Values.Contains(false);

        public bool IsUtilityWorking(UtilityType utilityType) => _states[utilityType];

        public void BreakUtility(UtilityType utilityType)
        {
            if (_states[utilityType])
            {
                _states[utilityType] = false;
                OnPropertyChanged(nameof(HasBrokenUtilities));
            }
        }

        public void FixUtility(UtilityType utilityType)
        {
            if (!_states[utilityType])
            {
                _states[utilityType] = true;
                OnPropertyChanged(nameof(HasBrokenUtilities));
            }
        }

        public IReadOnlyDictionary<UtilityType, bool> States => _states;
    }
}
