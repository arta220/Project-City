using Domain.Citizens;

namespace Services.CitizensSimulation
{
    public class CitizenSimulationService
    {
        private readonly CitizenController _controller;
        private readonly List<Citizen> allCitizens = new();

        public CitizenSimulationService(CitizenController controller)
        {
            _controller = controller;
        }

        public void AddCitizen(Citizen citizen)
        {
            allCitizens.Add(citizen);
        }

        public void UpdateAll()
        {
            foreach (var citizen in allCitizens)
                _controller.UpdateCitizen(citizen);
        }
    }

}
