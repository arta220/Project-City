using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Transports.Ground;
using Services.Common;
using Services.Time.Clock;
using System.Collections.ObjectModel;

namespace Services.TransportSimulation
{
    public class TransportSimulationService : IUpdatable
    {
        private readonly PersonalTransportController _controller;

        public TransportSimulationService(PersonalTransportController controller)
        {
            _controller = controller;
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

        public void Update(SimulationTime time)
        {
            foreach (var transport in Transports)
            {
                if (transport is PersonalCar car) // опнярн бпелеммюъ опнбепйю онрнл мюд внрн ядекюрэ   
                {
                    _controller.UpdateTransport(car, time);
                    TransportUpdated?.Invoke(transport);
                }
            }
        }
    }
}
