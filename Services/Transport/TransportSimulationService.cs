using Domain.Common.Time;
using Domain.Transports.Ground;
using Services.Common;
using Services.Time.Clock;
using System.Collections.ObjectModel;

namespace Services.Transport
{
    public class TransportSimulationService : IUpdatable
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

        public void AddCar(PersonalCar car)
        {
            if (car == null) return;
            Cars.Add(car);
            CarAdded?.Invoke(car);
        }

        public void RemoveCar(PersonalCar car)
        {
            if (car == null) return;
            Cars.Remove(car);
            CarRemoved?.Invoke(car);
        }

        public void Update(SimulationTime time)
        {
            foreach (var car in Cars)
            {
                _controller.UpdateCar(car, time);
                CarUpdated?.Invoke(car);
            }
        }
    }
}
