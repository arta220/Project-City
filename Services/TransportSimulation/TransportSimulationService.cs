using Domain.Common.Base;
using Domain.Common.Time;
using Services.Common;
using System;
using System.Collections.ObjectModel;
using Services.TransportSimulation;

namespace Services.TransportSimulation
{
    public class TransportSimulationService : IUpdatable
    {
        private readonly PersonalTransportController _controller;
        private readonly TransportMovementService _movementService;
        private bool _isPaused = true;

        public TransportSimulationService(PersonalTransportController controller, TransportMovementService movementService)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _movementService = movementService ?? throw new ArgumentNullException(nameof(movementService));
        }

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

        public void Update(Domain.Common.Time.SimulationTime time)
        {
            if (_isPaused) return;

            foreach (var transport in Transports)
            {
                // If personal car specific controller exists - use it
                if (transport is Domain.Transports.Ground.PersonalCar personal)
                {
                    _controller.UpdateTransport(personal, time);
                }

                // Movement logic for any transport
                _movementService.UpdateTransport(transport, time);

                TransportUpdated?.Invoke(transport);
            }
        }

        public void Resume() => _isPaused = false;
        public void Pause() => _isPaused = true;
    }
}
