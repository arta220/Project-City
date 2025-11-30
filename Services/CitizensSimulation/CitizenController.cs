using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Map;
using Domain.Time;
using Services.BuildingRegistry;
using Services.Interfaces;
using Services.Time;

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
        private readonly ISimulationTimeService _timeService;

        public CitizenController(
            IBuildingRegistry buildingRegistry,
            ICitizenMovementService movementService,
            IJobService jobService,
            IEducationService educationService,
            ISimulationTimeService timeService)
        {
            _buildingRegistry = buildingRegistry;
            _movement = movementService;
            _jobService = jobService;
            _educationService = educationService;
            _timeService = timeService;
        }

        /// <summary>
        /// Обновляет состояние указанного гражданина на текущем тике симуляции.
        /// </summary>
        /// <param name="citizen">Гражданин, состояние которого необходимо обновить.</param>
        /// <param name="tick">Текущий тик симуляции.</param>
        public void UpdateCitizen(Citizen citizen, SimulationTime time)
        {
            Placement? placement;
            bool found;
            Position pos;

            switch (citizen.State)
            {

                case CitizenState.Idle:
                    DecideNextAction(citizen);
                    break;

                case CitizenState.GoingToWork:
                    _movement.Move(citizen, new Position(0, 0), time); // Пока нет реализации работы жителя
                    break;

                case CitizenState.Working:
                    _jobService.UpdateWork(citizen, time);
                    break;

                case CitizenState.GoingToSchool:
                    (placement, found) = _buildingRegistry.TryGetPlacement(citizen.StudyPlace);

                    if (!found || placement is null)
                    {
                        citizen.State = CitizenState.Idle;
                        break;
                    }

                    pos = placement.Value.Position;

                    _movement.Move(citizen, pos, time);
                    break;

                case CitizenState.Studying:
                    _educationService.UpdateEducation(citizen, time);
                    break;

                case CitizenState.GoingHome:
                    (placement, found) = _buildingRegistry.TryGetPlacement(citizen.Home);

                    if (!found || placement is null)
                    {
                        citizen.State = CitizenState.Idle;
                        break;
                    }

                    pos = placement.Value.Position;

                    _movement.Move(citizen, pos, time);

                    if (citizen.Position.Equals(pos))
                    {
                        citizen.State = CitizenState.Idle;
                        citizen.CurrentPath.Clear();
                    }
                    break;


                /// КОММЕРЧЕСКИЕ ЗДАНИЯ
                /// 
                case CitizenState.WaitingInCommercialQueue:
                    HandleWaitingInCommercialQueue(citizen, time);
                    break;

                case CitizenState.UsingCommercialService:
                    HandleUsingCommercialService(citizen, time);
                    break;

                case CitizenState.LeavingCommercial:
                    HandleLeavingCommercial(citizen, time);
                    break;
            }
        }

        /// <summary>
        /// Определяет следующую цель гражданина в зависимости от текущих условий.
        /// </summary>
        /// <param name="citizen">Гражданин, для которого нужно определить действие.</param>
        /// <param name="tick">Текущий тик симуляции.</param>
        private void DecideNextAction(Citizen citizen)
        {
            if (!(_buildingRegistry.TryGetPlacement(citizen.Home)).found)
            {
                citizen.State = CitizenState.SearchingHome;
                return;
            }

            var timeOfDay = _timeService.GetTimeOfDay();
            var isWeekend = _timeService.IsWeekend();
            var isAtHome = IsAtHome(citizen);

            if (!isAtHome && (timeOfDay == TimeOfDay.Evening || _timeService.IsNightTime()))
            {
                citizen.State = CitizenState.GoingHome;
                return;
            }

            if (timeOfDay == TimeOfDay.Morning && !isWeekend && isAtHome)
            {
                if (NeedsEducation(citizen) && !IsAtSchool(citizen))
                {
                    citizen.State = CitizenState.GoingToSchool;
                    return;
                }
                else if (HasJob(citizen) && !IsAtWork(citizen))
                {
                    citizen.State = CitizenState.GoingToWork;
                    return;
                }
            }

            if ((citizen.State == CitizenState.Working || citizen.State == CitizenState.Studying) &&
                timeOfDay == TimeOfDay.Evening && !isAtHome)
            {
                citizen.State = CitizenState.GoingHome;
                return;
            }

            if (citizen.State == CitizenState.GoingToWork && IsAtWork(citizen))
            {
                citizen.State = CitizenState.Working;
                return;
            }

            if (citizen.State == CitizenState.GoingToSchool && IsAtSchool(citizen))
            {
                citizen.State = CitizenState.Studying;
                return;
            }

            if (isAtHome)
            {
                citizen.State = CitizenState.Idle;
                return;
            }
        }

        private bool IsAtHome(Citizen citizen)
        {
            var (placement, found) = _buildingRegistry.TryGetPlacement(citizen.Home);

            if (!found || placement == null)
                return false;

            Position pos = ((Placement)placement).Position;

            return citizen.Position.Equals(pos);
        }

        private bool IsAtWork(Citizen citizen)
        {
            var (placement, found) = _buildingRegistry.TryGetPlacement(citizen.WorkPlace);
            if (!found || placement == null)
                return false;

            Position pos = ((Placement)placement).Position;

            return citizen.Position.Equals(pos);
        }

        private bool IsAtSchool(Citizen citizen)
        {
            var (placement, found) = _buildingRegistry.TryGetPlacement(citizen.StudyPlace);
            if (!found || placement == null)
                return false;

            Position pos = ((Placement)placement).Position;

            return citizen.Position.Equals(pos);
        }

        private bool HasJob(Citizen citizen) => citizen.WorkPlace != null;
        private bool NeedsEducation(Citizen citizen) => citizen.StudyPlace != null;
    }
}
