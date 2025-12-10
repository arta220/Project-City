using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.EntityMovement.Service;

namespace Services.CitizensSimulation.Tasks
{
    /// <summary>
    /// Задача движения гражданина к транспорту.
    /// Гражданин движется к позиции транспорта.
    /// </summary>
    public class MoveToTransportTask : ICitizenTask
    {
        private readonly Transport _targetTransport;
        private readonly IEntityMovementService _movement;
        private bool _pathSet = false;
        
        public bool IsCompleted { get; private set; }

        public MoveToTransportTask(Transport targetTransport, IEntityMovementService movement)
        {
            _targetTransport = targetTransport ?? throw new ArgumentNullException(nameof(targetTransport));
            _movement = movement ?? throw new ArgumentNullException(nameof(movement));
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            // Если транспорт уже достигнут (гражданин рядом с транспортом)
            if (citizen.Position.Equals(_targetTransport.Position) || 
                citizen.Position.GetNeighbors().Any(n => n.Equals(_targetTransport.Position)))
            {
                IsCompleted = true;
                return;
            }

            // Устанавливаем маршрут к транспорту
            if (!_pathSet)
            {
                _movement.SetTarget(citizen, _targetTransport.Position);
                _pathSet = true;
            }

            // Выполняем движение
            _movement.PlayMovement(citizen, time);

            // Проверяем достижение цели
            if (citizen.Position.Equals(_targetTransport.Position) || 
                citizen.Position.GetNeighbors().Any(n => n.Equals(_targetTransport.Position)))
            {
                IsCompleted = true;
            }
        }
    }
}



