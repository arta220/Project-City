using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;

namespace Services.CitizensSimulation.Tasks
{
    /// <summary>
    /// Задача посадки гражданина в транспорт.
    /// Гражданин садится в транспорт и становится водителем или пассажиром.
    /// </summary>
    public class EnterTransportTask : ICitizenTask
    {
        private readonly Transport _transport;
        private bool _hasEntered = false;
        
        public bool IsCompleted { get; private set; }

        public EnterTransportTask(Transport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            // Проверяем, что гражданин рядом с транспортом
            if (!citizen.Position.Equals(_transport.Position) && 
                !citizen.Position.GetNeighbors().Any(n => n.Equals(_transport.Position)))
            {
                // Гражданин еще не дошел до транспорта
                return;
            }

            // Садимся в транспорт
            if (!_hasEntered)
            {
                citizen.CurrentTransport = _transport;
                
                // Если транспорт еще не имеет водителя, гражданин становится водителем
                if (_transport.Passengers.Count == 0)
                {
                    _transport.Passengers.Add(citizen);
                }
                else if (_transport.Passengers.Count < _transport.Capacity)
                {
                    // Если есть место, добавляем как пассажира
                    _transport.Passengers.Add(citizen);
                }

                _hasEntered = true;
            }

            // Задача завершена
            IsCompleted = true;
        }
    }
}



