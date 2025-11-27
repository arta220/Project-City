using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Interfaces;

namespace Services.CitizensSimulation
{
    /// <summary>
    /// Контроллер для управления поведением граждан в симуляции.
    /// Отвечает за обновление состояния граждан, перемещение по карте, работу, обучение и старение.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется внутри <see cref="CitizenSimulationService"/> для обновления каждого гражданина на каждом тике симуляции.
    /// - Использует <see cref="IBuildingRegistry"/> для получения позиций зданий (дом, работа, школа и др.).
    /// - Использует сервисы движения, работы, образования и демографии (<see cref="ICitizenMovementService"/>, <see cref="IJobService"/>, <see cref="IEducationService"/>, <see cref="IPopulationService"/>).
    /// - MapVM косвенно получает обновлённые позиции граждан через <see cref="CitizenManagerService"/>.
    /// 
    /// Возможные расширения:
    /// - Реализовать фактическую логику поиска работы и школы.
    /// - Добавить события или callback'и для визуальных эффектов (например, анимация движения на карте).
    /// - Поддерживать дополнительные состояния граждан (развлечения, покупки, болезни и т.п.).
    /// </remarks>
    public class CitizenController
    {
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly ICitizenMovementService _movement;
        private readonly IJobService _jobService;
        private readonly IEducationService _educationService;
        private readonly IPopulationService _populationService;

        /// <summary>
        /// Создаёт контроллер граждан с зависимостями на сервисы симуляции.
        /// </summary>
        /// <param name="buildingRegistry">Сервис для получения размещений зданий.</param>
        /// <param name="movementService">Сервис перемещения граждан по карте.</param>
        /// <param name="jobService">Сервис для обновления работы граждан.</param>
        /// <param name="educationService">Сервис для обновления обучения граждан.</param>
        /// <param name="populationService">Сервис для управления возрастом и демографией граждан.</param>
        public CitizenController(
              IBuildingRegistry buildingRegistry,
              ICitizenMovementService movementService,
              IJobService jobService,
              IEducationService educationService,
              IPopulationService populationService)
        {
            _buildingRegistry = buildingRegistry;
            _movement = movementService;
            _jobService = jobService;
            _educationService = educationService;
            _populationService = populationService;
        }

        /// <summary>
        /// Обновляет состояние указанного гражданина на текущем тике симуляции.
        /// </summary>
        /// <param name="citizen">Гражданин, состояние которого необходимо обновить.</param>
        /// <param name="tick">Текущий тик симуляции.</param>
        public void UpdateCitizen(Citizen citizen, int tick)
        {
            switch (citizen.State)
            {
                case CitizenState.Idle:
                    DecideNextAction(citizen, tick);
                    break;

                case CitizenState.GoingToWork:
                    _movement.Move(citizen, new Position(0, 0), tick); // Пока нет реализации работы жителя
                    break;

                case CitizenState.Working:
                    _jobService.UpdateWork(citizen, tick);
                    break;

                case CitizenState.GoingToSchool:
                    _movement.Move(citizen, new Position(0, 0), tick); // Пока нет реализации учёбы
                    break;

                case CitizenState.Studying:
                    _educationService.UpdateEducation(citizen, tick);
                    break;

                case CitizenState.GoingHome:
                    var homePos = _buildingRegistry.GetPlacement(citizen.Home).Position;
                    _movement.Move(citizen, homePos, tick);

                    if (citizen.Position.Equals(homePos))
                    {
                        citizen.State = CitizenState.Idle;
                        citizen.CurrentPath.Clear();
                    }
                    break;
            }

            _populationService.AgeCitizen(citizen);
        }

        /// <summary>
        /// Определяет следующую цель гражданина в зависимости от текущих условий.
        /// </summary>
        /// <param name="citizen">Гражданин, для которого нужно определить действие.</param>
        /// <param name="tick">Текущий тик симуляции.</param>
        private void DecideNextAction(Citizen citizen, int tick)
        {

        }
    }
}
