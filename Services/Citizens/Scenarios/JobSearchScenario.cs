using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.EntityMovement.Service;
using Services.Interfaces;
using Services.Time;

namespace Services.Citizens.Scenarios
{
    public class JobSearchScenario : ICitizenScenario
    {
        private readonly IFindJobService _findJobService;
        private readonly IEntityMovementService _movementService;
        private readonly IBuildingRegistry _registry;
        private readonly ISimulationTimeService _timeService;

        public JobSearchScenario(
            IFindJobService findJobService,
            IEntityMovementService movementService,
            IBuildingRegistry registry,
            ISimulationTimeService timeService)
        {
            _findJobService = findJobService;
            _movementService = movementService;
            _registry = registry;
            _timeService = timeService;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Проверяем, что жителю пора искать работу
            if (citizen.Age < 18 || citizen.Age > 65)
                return false; // Несовершеннолетние или пенсионеры не ищут работу

            if (citizen.WorkPlace != null)
                return false; // Уже есть работа

            if (citizen.Profession == CitizenProfession.Unemployed)
                return false; // Нет профессии (возможно, нужно будет реализовать)

            // Ищем работу только в рабочее время
            //if (!_timeService.IsWorkTime())
            //    return false;

            // Проверяем, что у жителя нет текущих задач или он idle
            //if (citizen.CurrentTask != null || citizen.Tasks.Count > 0)
            //    return false;

            // Проверяем состояние
            if (citizen.State != CitizenState.Idle)
                return false;

            return true;
        }

        public void BuildTasks(Citizen citizen)
        {
            // Ищем доступные вакансии для профессии жителя
            var availableJobs = _findJobService.FindJob(citizen.Profession).ToList();

            if (availableJobs.Count == 0)
            {
                // Нет доступных вакансий - остался без работы
                citizen.State = CitizenState.Idle;
                return;
            }

            // Выбираем ближайшую вакансию (можно добавить более сложную логику выбора)
            var nearestJob = FindNearestJob(citizen, availableJobs);

            if (nearestJob == null)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            // Устанавливаем состояние и создаем задачи
            citizen.State = CitizenState.LookingForJob;

            // 1. Задача: дойти до места работы
            citizen.Tasks.Enqueue(
                new MoveToBuildingTask(nearestJob, _movementService, _registry)
            );

            // 2. Задача: устроиться на работу
            citizen.Tasks.Enqueue(
                new ApplyForJobTask(nearestJob)
            );
        }

        private Building? FindNearestJob(Citizen citizen, List<Building> availableJobs)
        {
            // Простая реализация - выбираем первую доступную вакансию
            // Можно улучшить, добавив расчет расстояния или приоритет зданий
            return availableJobs.FirstOrDefault();
        }
    }
}