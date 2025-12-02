using CitySimulatorWPF.ViewModels;
using Domain.Common.Base;
using Services.TransportSimulation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace CitySimulatorWPF.Services
{
    public interface ICarManagerService
    {
        ObservableCollection<PersonalCarVM> Cars { get; }

        void StartSimulation(TransportSimulationService simulation);
        void StopSimulation();
    }

    public class CarManagerService : ICarManagerService
    {
        private TransportSimulationService? _simulation;

        private readonly ObservableCollection<PersonalCarVM> _cars;
        public ObservableCollection<PersonalCarVM> Cars => _cars;

        public CarManagerService()
        {
            _cars = new ObservableCollection<PersonalCarVM>();
            BindingOperations.EnableCollectionSynchronization(_cars, new object());
        }

        public void StartSimulation(TransportSimulationService simulation)
        {
            if (simulation == null) throw new ArgumentNullException(nameof(simulation));

            _simulation = simulation;

            foreach (var car in _simulation.Transports)
            {
                _cars.Add(new PersonalCarVM(car));
            }

            _simulation.TransportAdded += OnCarAdded;
            _simulation.TransportRemoved += OnCarRemoved;
            _simulation.TransportUpdated += OnCarUpdated;
        }

        public void StopSimulation()
        {
            if (_simulation == null) return;

            _simulation.TransportAdded -= OnCarAdded;
            _simulation.TransportRemoved -= OnCarRemoved;
            _simulation.TransportUpdated -= OnCarUpdated;

            _cars.Clear();
            _simulation = null;
        }

        private void OnCarAdded(Transport car)
        {
            _cars.Add(new PersonalCarVM(car));
        }

        private void OnCarRemoved(Transport car)
        {
            var vm = _cars.FirstOrDefault(c => c.Car == car);
            if (vm != null)
                _cars.Remove(vm);
        }

        private void OnCarUpdated(Transport car)
        {
            var vm = _cars.FirstOrDefault(c => c.Car == car);
            vm?.UpdatePosition();
        }
    }
}
