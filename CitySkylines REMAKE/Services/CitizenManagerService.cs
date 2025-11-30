using CitySimulatorWPF.ViewModels;
using Domain.Citizens;
using Services.CitizensSimulation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace CitySimulatorWPF.Services
{
    public interface ICitizenManagerService
    {
        ObservableCollection<CitizenVM> Citizens { get; }

        void StartSimulation(CitizenSimulationService simulation);

        void StopSimulation();
    }
    public class CitizenManagerService : ICitizenManagerService
    {
        private CitizenSimulationService _simulation;


        private readonly ObservableCollection<CitizenVM> _citizens;
        public ObservableCollection<CitizenVM> Citizens => _citizens;

        public CitizenManagerService()
        {
            _citizens = new ObservableCollection<CitizenVM>();
            BindingOperations.EnableCollectionSynchronization(_citizens, new object());
        }
        public void StartSimulation(CitizenSimulationService simulation)
        {
            if (simulation == null) throw new ArgumentNullException(nameof(simulation));

            _simulation = simulation;

            foreach (var citizen in _simulation.Citizens)
            {
                Citizens.Add(new CitizenVM(citizen));
            }

            _simulation.CitizenAdded += OnCitizenAdded;
            _simulation.CitizenRemoved += OnCitizenRemoved;
            _simulation.CitizenUpdated += OnCitizenUpdated;
        }

        public void StopSimulation()
        {
            if (_simulation == null) return;

            _simulation.CitizenAdded -= OnCitizenAdded;
            _simulation.CitizenRemoved -= OnCitizenRemoved;
            _simulation.CitizenUpdated -= OnCitizenUpdated;

            Citizens.Clear();
            _simulation = null;
        }

        private void OnCitizenAdded(Citizen citizen)
        {
            Citizens.Add(new CitizenVM(citizen));
        }

        private void OnCitizenRemoved(Citizen citizen)
        {
            var vm = Citizens.FirstOrDefault(c => c.Citizen == citizen);
            if (vm != null)
                Citizens.Remove(vm);
        }

        private void OnCitizenUpdated(Citizen citizen)
        {
            var vm = Citizens.FirstOrDefault(c => c.Citizen == citizen);
            vm?.UpdatePosition();
        }
    }
}
