using Domain.Map;
using Domain.Transports.Ground;
using Services.SimulationClock;
using System;
using System.Collections.ObjectModel;

namespace Services.CitizensSimulation
{
    /// <summary>
    /// Сервис симуляции личного транспорта (машин).
    /// По аналогии с CitizenSimulationService обновляет все машины на каждом тике.
    /// </summary>
    public class TransportSimulationService
    {
        private readonly TransportController _controller;
        private readonly ISimulationClock _clock;

        public ObservableCollection<PersonalCar> Cars { get; } = new();

        public event Action<PersonalCar>? CarAdded;
        public event Action<PersonalCar>? CarRemoved;
        public event Action<PersonalCar>? CarUpdated;

        public TransportSimulationService(
            TransportController controller,
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

        public void AddCar(PersonalCar car)
        {
            if (car == null) throw new ArgumentNullException(nameof(car));

            Cars.Add(car);
            CarAdded?.Invoke(car);
        }

        public void RemoveCar(PersonalCar car)
        {
            if (Cars.Remove(car))
            {
                CarRemoved?.Invoke(car);
            }
        }

        /// <summary>
        /// Обновляет все машины на текущем тике, используя контроллер поведения.
        /// </summary>
        public void UpdateAll(int tick)
        {
            foreach (var car in Cars)
            {
                _controller.UpdateCar(car, tick);
                CarUpdated?.Invoke(car);
            }
        }
    }
}
