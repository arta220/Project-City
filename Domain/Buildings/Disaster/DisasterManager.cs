using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Enums;

namespace Domain.Buildings.Disaster
{
    public class DisasterManager : ObservableObject
    {
        private readonly Dictionary<DisasterType, bool> _states;
        private readonly Dictionary<DisasterType, int> _startTicks;
        private bool _isBeingHandled; // ← ДОБАВИТЬ

        public DisasterManager()
        {
            _states = new Dictionary<DisasterType, bool>
            {
                [DisasterType.Fire] = false,
                [DisasterType.Flood] = false,
                [DisasterType.Blizzard] = false
            };

            _startTicks = new Dictionary<DisasterType, int>();
        }

        public bool HasDisaster => _states.Values.Contains(true);
        public bool IsBeingHandled => _isBeingHandled; // ← ДОБАВИТЬ

        public bool IsDisasterActive(DisasterType disasterType) => _states[disasterType];

        public void StartDisaster(DisasterType disasterType, int currentTick)
        {
            // Stop all other disasters before starting a new one
            var activeDisasters = _states.Where(s => s.Value).Select(s => s.Key).ToList();
            foreach (var activeDisaster in activeDisasters)
            {
                StopDisaster(activeDisaster);
            }

            if (!_states[disasterType])
            {
                _states[disasterType] = true;
                _startTicks[disasterType] = currentTick;
                _isBeingHandled = false; // ← ДОБАВИТЬ
                OnPropertyChanged(nameof(HasDisaster));
                OnPropertyChanged(nameof(IsBeingHandled)); // ← ДОБАВИТЬ
            }
        }

        public void StopDisaster(DisasterType disasterType)
        {
            if (_states[disasterType])
            {
                _states[disasterType] = false;
                _startTicks.Remove(disasterType);
                _isBeingHandled = false; // ← ДОБАВИТЬ
                OnPropertyChanged(nameof(HasDisaster));
                OnPropertyChanged(nameof(IsBeingHandled)); // ← ДОБАВИТЬ
            }
        }

        public IReadOnlyDictionary<DisasterType, bool> States => _states;

        public int GetStartTick(DisasterType type)
        {
            return _startTicks.ContainsKey(type) ? _startTicks[type] : 0;
        }
    }
}