// Domain/Buildings/Disaster/DisasterManager.cs
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Enums;
using System.Linq;

namespace Domain.Buildings.Disaster
{
    public class DisasterManager : ObservableObject
    {
        private readonly Dictionary<DisasterType, bool> _states;
        private readonly Dictionary<DisasterType, int> _startTicks;

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

        public bool IsDisasterActive(DisasterType disasterType) => _states[disasterType];

        public void StartDisaster(DisasterType disasterType, int currentTick)
        {
            // Останавливаем все предыдущие бедствия, если они есть
            var existingDisasters = _states.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
            foreach (var existingDisaster in existingDisasters)
            {
                StopDisaster(existingDisaster);
            }

            // Запускаем новое бедствие
            if (!_states[disasterType])
            {
                _states[disasterType] = true;
                _startTicks[disasterType] = currentTick;
                OnPropertyChanged(nameof(HasDisaster));
            }
        }

        public void StopDisaster(DisasterType disasterType)
        {
            if (_states[disasterType])
            {
                _states[disasterType] = false;
                _startTicks.Remove(disasterType);
                OnPropertyChanged(nameof(HasDisaster));
            }
        }

        public IReadOnlyDictionary<DisasterType, bool> States => _states;

        public int GetStartTick(DisasterType type)
        {
            return _startTicks.ContainsKey(type) ? _startTicks[type] : 0;
        }
    }
}

