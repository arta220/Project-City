using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Domain.Map;
using Services.Citizens.Movement;
using System.Net.Http.Headers;

public class CitizenController
{
    private readonly ICitizenMovementService _movementService;
    public CitizenController(ICitizenMovementService movementService)
    {
        _movementService = movementService;
    }

    public void UpdateCitizen(Citizen citizen, SimulationTime time)
    {
        // Выбираем текущую задачу, если её ещё нет
        if (citizen.CurrentTask == null)
        {
            if (citizen.Tasks.Count > 0)
            {
                citizen.CurrentTask = citizen.Tasks.Dequeue();
            }
        }

        // Если есть задача и нет пути — настраиваем цель для пешего движения
        if (citizen.CurrentTask != null && citizen.CurrentPath.Count == 0)
        {
            var task = citizen.CurrentTask;

            // Пешие задачи: идём к точке
            if (task.TaskType == CitizenTaskType.MoveToPosition ||
                task.TaskType == CitizenTaskType.WalkToCar)
            {
                _movementService.SetTarget(citizen, task.Target);
            }
        }

        // Двигаем жителя пешком, если есть путь
        _movementService.PlayMovement(citizen, time);

        // Проверяем выполнение текущей задачи
        if (citizen.CurrentTask != null)
        {
            var task = citizen.CurrentTask;

            // Завершение пеших задач, когда достигнута цель
            if ((task.TaskType == CitizenTaskType.MoveToPosition ||
                 task.TaskType == CitizenTaskType.WalkToCar) &&
                citizen.Position == task.Target)
            {
                task.MarkAsCompleted();
                citizen.CurrentTask = null;
                return;
            }

            // Задача посадки в машину: житель уже дошёл до машины
            if (task.TaskType == CitizenTaskType.EnterCar &&
                citizen.PersonalCar != null &&
                citizen.Position == citizen.PersonalCar.Position)
            {
                // Садимся в машину
                citizen.CurrentTransport = citizen.PersonalCar;
                citizen.PersonalCar.Passengers.Add(citizen);

                // Устанавливаем машине цель — точка из задачи (вход в работу или в дом)
                citizen.PersonalCar.TargetPosition = task.Target;

                // Направление движения: если сейчас рабочее время — едем на работу, иначе едем домой
                if (time.IsWorkTime)
                {
                    citizen.PersonalCar.State = Domain.Transports.States.TransportState.DrivingToWork;
                }
                else
                {
                    citizen.PersonalCar.State = Domain.Transports.States.TransportState.DrivingHome;
                }

                task.MarkAsCompleted();
                citizen.CurrentTask = null;
                return;
            }
        }
    }
}
