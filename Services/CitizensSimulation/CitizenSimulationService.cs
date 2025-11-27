using Domain.Citizens;
using Services.SimulationClock;

namespace Services.CitizensSimulation
{
    public class CitizenSimulationService
    {
        private readonly CitizenController _controller;
        public readonly List<Citizen> Citizens = new();

        public event Action<Citizen> CitizenAdded;
        public event Action<Citizen> CitizenRemoved;
        public event Action<Citizen> CitizenUpdated;

        public CitizenSimulationService(
            CitizenController controller,
            ISimulationClock clock)
        {
            clock.TickOccurred += UpdateAll;
            _controller = controller;
        }

        public void AddCitizen(Citizen citizen)
        {
            Citizens.Add(citizen);
            CitizenAdded?.Invoke(citizen);
        }

        public void UpdateAll(int tick)
        {
            foreach (var citizen in Citizens)
            {
                _controller.UpdateCitizen(citizen, tick);
            }
        }
    }

}
