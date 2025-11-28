using Domain.Base;
using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Interfaces;
using System.Collections.Generic; 
using System.Linq;

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
        private readonly System.Random _random; // ve1ce - коммерция

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
            _random = new System.Random(); // ve1ce - коммерция
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
                    var (placement, found) = _buildingRegistry.TryGetPlacement(citizen.Home);

                    if (!found || placement is null)
                    {
                        citizen.State = CitizenState.SearchingHome; // ну если дом удалили надо типа найти новый
                        break;
                    }

                    Position pos = ((Placement)placement).Position;

                    _movement.Move(citizen, pos, tick);

                    if (citizen.Position.Equals(pos))
                    {
                        citizen.State = CitizenState.Idle;
                        citizen.CurrentPath.Clear();
                    }
                    break;

                case CitizenState.GoingToCommercial: // ve1ce - коммерция
                    HandleGoingToCommercial(citizen, tick);
                    break;

                case CitizenState.UsingCommercialService:  // ve1ce - коммерция (сомневаюсь что это нужно)
                    // Гражданин уже в здании, ничего не делаем - здание само обработает тик
                    break;
            }

            _populationService.AgeCitizen(citizen);
        }

        /// <summary>
        /// Определяет следующую цель гражданина в зависимости от текущих условий.
        /// </summary>
        private void DecideNextAction(Citizen citizen, int tick)// ve1ce - коммерция
        {
            // Сбрасываем предыдущую цель когда решаем новое действие
            citizen.CurrentPath.Clear();

            // Случайное решение пойти в коммерческое здание (15% chance)
            if (_random.Next(100) < 15)
            {
                var commercialBuildings = GetAvailableCommercialBuildings();
                if (commercialBuildings.Any())
                {
                    var targetBuilding = commercialBuildings[_random.Next(commercialBuildings.Count)];

                    // Получаем позицию здания из реестра
                    var (placement, found) = _buildingRegistry.TryGetPlacement(targetBuilding as MapObject);
                    if (found && placement.HasValue)
                    {
                        citizen.State = CitizenState.GoingToCommercial;

                        // Центр здания
                        var centerPosition = new Position(
                            placement.Value.Position.X + placement.Value.Area.Width / 2,
                            placement.Value.Position.Y + placement.Value.Area.Height / 2
                        );
                        citizen.TargetPosition = centerPosition;
                        return;
                    }
                }
            }

            // Логика для других действий...
        }

        /// <summary>
        /// Обрабатывает перемещение гражданина к коммерческому зданию.
        /// </summary>
        private void HandleGoingToCommercial(Citizen citizen, int tick) // ve1ce - коммерция
        {
            var targetBuildings = GetCommercialBuildingAtPosition(citizen.TargetPosition);

            if (!targetBuildings.Any())
            {
                citizen.State = CitizenState.Idle;
                citizen.CurrentPath.Clear();
                return;
            }

            _movement.Move(citizen, citizen.TargetPosition, tick);

            if (citizen.Position.Equals(citizen.TargetPosition))
            {
                var commercialBuildings = GetCommercialBuildingAtPosition(citizen.TargetPosition);
                if (commercialBuildings.Any())
                {
                    var targetBuilding = commercialBuildings.First();
                    targetBuilding.EnqueueCitizen(citizen);
                    citizen.State = CitizenState.UsingCommercialService;
                    citizen.CurrentPath.Clear();
                }
                else
                {
                    citizen.State = CitizenState.Idle;
                }
            }
        }

        /// <summary>
        /// Получает список доступных коммерческих зданий.
        /// </summary>
        private List<IServiceBuilding> GetAvailableCommercialBuildings() // ve1ce - коммерция
        {
            var allBuildings = _buildingRegistry.GetAllBuildings();
            var commercialBuildings = new List<IServiceBuilding>();

            foreach (var building in allBuildings)
            {
                if (building is IServiceBuilding serviceBuilding && serviceBuilding.CanAcceptMoreVisitors)
                {
                    commercialBuildings.Add(serviceBuilding);
                }
            }

            return commercialBuildings;
        }

        /// <summary>
        /// Получает коммерческие здания в указанной позиции.
        /// </summary>
        private List<IServiceBuilding> GetCommercialBuildingAtPosition(Position position) // ve1ce - коммерция
        {
            var allBuildings = _buildingRegistry.GetAllBuildings();
            var commercialBuildings = new List<IServiceBuilding>();

            foreach (var building in allBuildings)
            {
                if (building is IServiceBuilding serviceBuilding)
                {
                    var (placement, found) = _buildingRegistry.TryGetPlacement(building);
                    if (found && placement.HasValue && placement.Value.Contains(position))
                    {
                        commercialBuildings.Add(serviceBuilding);
                    }
                }
            }

            return commercialBuildings;
        }
    }
}
