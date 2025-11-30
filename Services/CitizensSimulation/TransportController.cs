using Domain.Citizens.States;
using Domain.Transports.Ground;
using Domain.Transports.States;

namespace Services.CitizensSimulation
{
    /// <summary>
    /// Контроллер поведения личного транспорта.
    /// Определяет, куда должна ехать машина, и вызывает сервис движения.
    /// </summary>
    public class TransportController
    {
        private readonly TransportMovementService _movementService;

        /// <summary>
        /// Сколько тиков машина должна простоять у работы, прежде чем поедет домой.
        /// Значение задаётся через конструктор, чтобы можно было настраивать "длину рабочего дня".
        /// </summary>
        private readonly int _ticksToStayAtWork;

        public TransportController(TransportMovementService movementService, int ticksToStayAtWork)
        {
            _movementService = movementService;
            _ticksToStayAtWork = ticksToStayAtWork;
        }

        /// <summary>
        /// Обновляет состояние и позицию машины на текущем тике симуляции.
        /// </summary>
        public void UpdateCar(PersonalCar car, int tick)
        {
            switch (car.State)
            {
                case TransportState.IdleAtHome:
                    // Простая логика: как только симуляция идёт, машина поедет на работу
                    if (car.WorkPosition != default)
                    {
                        car.State = TransportState.DrivingToWork;
                        _movementService.Move(car, car.WorkPosition);
                    }
                    break;

                case TransportState.DrivingToWork:
                    _movementService.Move(car, car.WorkPosition);

                    if (car.Position.Equals(car.WorkPosition))
                    {
                        car.CurrentPath.Clear();
                        car.State = TransportState.ParkedAtWork;

                        if (car.Owner != null)
                        {
                            car.Owner.State = CitizenState.Working;
                        }
                    }
                    break;

                case TransportState.ParkedAtWork:
                    // Машина стоит у работы; по прошествии нужного числа тиков едет домой.
                    car.TicksAtWork++;

                    if (car.TicksAtWork >= _ticksToStayAtWork)
                    {
                        car.TicksAtWork = 0;
                        car.State = TransportState.DrivingHome;
                        _movementService.Move(car, car.HomePosition);

                        if (car.Owner != null)
                        {
                            car.Owner.State = CitizenState.GoingHome;
                        }
                    }
                    break;

                case TransportState.DrivingHome:
                    _movementService.Move(car, car.HomePosition);

                    if (car.Position.Equals(car.HomePosition))
                    {
                        car.CurrentPath.Clear();
                        car.State = TransportState.IdleAtHome;

                        if (car.Owner != null)
                        {
                            car.Owner.State = CitizenState.Idle;
                        }
                    }
                    break;
            }

            // Если есть владелец, "везём" его вместе с машиной
            if (car.Owner != null)
            {
                car.Owner.Position = car.Position;
            }
        }
    }
}
