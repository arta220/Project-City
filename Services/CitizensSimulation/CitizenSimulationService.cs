using Domain.Citizens;
using Services.SimulationClock;

namespace Services.CitizensSimulation
{
    public class CitizenSimulationService
    {
        private readonly CitizenController _controller;
        private readonly List<Citizen> allCitizens = new();

        public CitizenSimulationService(
            CitizenController controller,
            ISimulationClock clock)
        {
            clock.TickOccurred += UpdateAll;
            _controller = controller;
        }

        public void AddCitizen(Citizen citizen)
        {
            allCitizens.Add(citizen);
        }

        public void UpdateAll(int tick)
        {
            foreach (var citizen in allCitizens)
                _controller.UpdateCitizen(citizen, tick);
        }
    }

}
