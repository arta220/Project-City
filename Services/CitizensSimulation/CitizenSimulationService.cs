using Domain.Citizens;
using Services.SimulationClock;
using System;
using System.Collections.ObjectModel;

namespace Services.CitizensSimulation
{
    public class CitizenSimulationService
    {
        private readonly CitizenController _controller;
        private readonly ISimulationClock _clock;

        public ObservableCollection<Citizen> Citizens { get; } = new();

        public event Action<Citizen> CitizenAdded;
        public event Action<Citizen> CitizenRemoved;
        public event Action<Citizen> CitizenUpdated;

        public CitizenSimulationService(
            CitizenController controller,
            ISimulationClock clock)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public void Start()
        {
            _clock.TickOccurred += UpdateAll;
        }

        public void Stop()
        {
            _clock.TickOccurred -= UpdateAll;
        }

        public void AddCitizen(Citizen citizen)
        {
            if (citizen == null) throw new ArgumentNullException(nameof(citizen));

            Citizens.Add(citizen);
            CitizenAdded?.Invoke(citizen);
        }

        public void RemoveCitizen(Citizen citizen)
        {
            if (Citizens.Remove(citizen))
            {
                CitizenRemoved?.Invoke(citizen);
            }
        }

        public void UpdateAll(int tick)
        {
            foreach (var citizen in Citizens)
            {
                _controller.UpdateCitizen(citizen, tick);
                CitizenUpdated?.Invoke(citizen);
            }
        }
    }
}
