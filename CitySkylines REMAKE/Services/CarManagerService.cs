using CitySimulatorWPF.ViewModels;
using Domain.Transports.Ground;
using Services.CitizensSimulation;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CitySimulatorWPF.Services
{
    public interface ICarManagerService
    {
        ObservableCollection<PersonalCarVM> Cars { get; }

        void StartSimulation(TransportSimulationService simulation);
        void StopSimulation();
    }

    /// <summary>
    /// Сервис, который связывает симуляцию машин с их визуальными представлениями.
    /// </summary>
    public class CarManagerService : ICarManagerService
    {
        private TransportSimulationService? _simulation;

        public ObservableCollection<PersonalCarVM> Cars { get; } = new();

        public void StartSimulation(TransportSimulationService simulation)
        {
            if (simulation == null) throw new ArgumentNullException(nameof(simulation));

            _simulation = simulation;

            foreach (var car in _simulation.Cars)
            {
                Cars.Add(new PersonalCarVM(car));
            }

            _simulation.CarAdded += OnCarAdded;
            _simulation.CarRemoved += OnCarRemoved;
            _simulation.CarUpdated += OnCarUpdated;
        }

        public void StopSimulation()
        {
            if (_simulation == null) return;

            _simulation.CarAdded -= OnCarAdded;
            _simulation.CarRemoved -= OnCarRemoved;
            _simulation.CarUpdated -= OnCarUpdated;

            Cars.Clear();
            _simulation = null;
        }

        private void OnCarAdded(PersonalCar car)
        {
            Cars.Add(new PersonalCarVM(car));
        }

        private void OnCarRemoved(PersonalCar car)
        {
            var vm = Cars.FirstOrDefault(c => c.Car == car);
            if (vm != null)
                Cars.Remove(vm);
        }

        private void OnCarUpdated(PersonalCar car)
        {
            var vm = Cars.FirstOrDefault(c => c.Car == car);
            vm?.UpdatePosition();
        }
    }
}
