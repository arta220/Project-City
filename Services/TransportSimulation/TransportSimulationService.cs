using Domain.Citizens;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Domain.Transports;
using Domain.Transports.Ground;
using Domain.Transports.States;
using Services.Common;
using Services.EntityMovement.Profile;
using Services.Time.Clock;
using Services.TransportSimulation.RideSessions;
using System;
using System.Collections.ObjectModel;

namespace Services.TransportSimulation
{
    public class TransportSimulationService : IUpdatable
    {
        private readonly PersonalTransportController _controller;
        private readonly RideSessionService _rideSessionService;
        private bool _isPaused = true;

        public TransportSimulationService(PersonalTransportController controller, RideSessionService rideSessionService)
        {
            _controller = controller;
            _rideSessionService = rideSessionService ?? throw new ArgumentNullException(nameof(rideSessionService));
        }

        public RideSessionService RideSessionService => _rideSessionService;

        public ObservableCollection<Transport> Transports { get; } = new();

        public event Action<Transport>? TransportAdded;
        public event Action<Transport>? TransportRemoved;
        public event Action<Transport>? TransportUpdated;

        public void AddTransport(Transport transport)
        {
            Transports.Add(transport);
            TransportAdded?.Invoke(transport);
        }

        public void RemoveTransport(Transport transport)
        {
            Transports.Remove(transport);
            TransportRemoved?.Invoke(transport);
        }

        public void Update(SimulationTime time)
        {
            if (_isPaused) return;

            // Обновляем сессии поездок (движение транспорта)
            _rideSessionService.Update(time);

            foreach (var transport in Transports)
            {
                if (transport is PersonalCar car) 
                {
                    _controller.UpdateTransport(car, time);
                    TransportUpdated?.Invoke(transport);
                }
            }
        }

        /// <summary>
        /// Создает автомобиль для гражданина
        /// </summary>
        public PersonalCar CreatePersonalCar(Citizen owner, MapModel map)
        {
            var car = new PersonalCar(new Area(1, 1), speed: 1.5f, owner: owner)
            {
                NavigationProfile = new TransportProfile(map, TransportType.Car)
            };

            // Размещаем автомобиль дома у владельца
            car.Position = owner.HomePosition;

            AddTransport(car);
            return car;
        }

        /// <summary>
        /// Начинает поездку водителя
        /// </summary>
        public IDriverRideSession StartDriverTrip(Citizen driver, Position destination)
        {
            if (driver.PersonalCar == null)
                throw new InvalidOperationException("У гражданина нет автомобиля");

            var car = driver.PersonalCar;

            // Проверяем, что автомобиль доступен
            if (car.CurrentDriver != null && car.CurrentDriver != driver)
                throw new InvalidOperationException("Автомобиль уже используется");

            // Настраиваем автомобиль для поездки
            car.State = TransportState.DrivingToTarget;
            car.AddTarget(destination);
            car.CurrentDriver = driver;
            car.IsDriving = true;

            // Создаем сессию поездки
            return _rideSessionService.CreateDriverRideSession(driver, car, destination);
        }

        /// <summary>
        /// Начинает поездку пассажира
        /// </summary>
        public IPassengerRideSession StartPassengerTrip(Citizen passenger, Transport transport, Position exitStop)
        {
            // Проверяем, что транспорт может принять пассажира
            if (transport.Passengers.Count >= transport.Capacity)
                throw new InvalidOperationException("В транспорте нет свободных мест");

            // Сажаем пассажира
            transport.Passengers.Add(passenger);
            passenger.CurrentTransport = transport;

            // Создаем сессию поездки пассажира
            return _rideSessionService.CreatePassengerRideSession(passenger, exitStop);
        }

        public void Resume()
        {
            _isPaused = false;
        }

        public void Pause()
        {
            _isPaused = true;
        }
    }
}
