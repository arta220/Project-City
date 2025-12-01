using Domain.Common.Time;
using Domain.Transports.Ground;
using Domain.Transports.States;
using Services.BuildingRegistry;

namespace Services.Transport
{
    /// <summary>
    /// Контроллер поведения личного транспорта.
    /// Определяет, куда должна ехать машина, и вызывает сервис движения.
    /// </summary>
    public class TransportController
    {
        private readonly TransportMovementService _movementService;
        private readonly IBuildingRegistry _buildingRegistry;
        /// <summary>
        /// Сколько тиков машина должна простоять у работы, прежде чем поедет домой.
        /// Значение задаётся через конструктор, чтобы можно было настраивать "длину рабочего дня".
        /// </summary>
        private readonly int _ticksToStayAtWork;

        public TransportController(
            TransportMovementService movementService, 
            IBuildingRegistry buildingRegistry,
            int ticksToStayAtWork)
        {
            _buildingRegistry = buildingRegistry;
            _movementService = movementService;
            _ticksToStayAtWork = ticksToStayAtWork;
        }

        /// <summary>
        /// Обновляет состояние и позицию машины на текущем тике симуляции.
        /// </summary>
        public void UpdateCar(PersonalCar car, SimulationTime time)
        {
            if (car.TargetPosition != null)
            {
                _movementService.Move(car, car.TargetPosition);

                if (car.Position.Equals(car.TargetPosition))
                {
                    car.CurrentPath.Clear();

                    switch (car.State)
                    {
                        case TransportState.DrivingToWork:
                            car.State = TransportState.ParkedAtWork;
                            break;
                        case TransportState.DrivingHome:
                            car.State = TransportState.IdleAtHome;
                            break;
                    }
                }
            }

            if (car.Owner != null)
            {
                car.Owner.Position = car.Position;
            }

            if (car.State == TransportState.ParkedAtWork)
            {
                car.TicksAtWork++;
                if (car.TicksAtWork >= _ticksToStayAtWork)
                {
                    car.TicksAtWork = 0;
                    car.State = TransportState.DrivingHome;

                    if (car.Owner != null)
                    {
                        var (placement, found) = _buildingRegistry.TryGetPlacement(car.Owner.Home);
                        if (found && placement != null)
                            car.TargetPosition = placement.Value.Position;
                    }
                }
            }
        }
    }
}
