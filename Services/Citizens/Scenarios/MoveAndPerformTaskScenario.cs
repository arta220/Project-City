using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks.Move;
using Services.EntityMovement.Service;
using Services.Time;

namespace Services.Citizens.Scenarios.Base
{
    /// <summary>
    /// Базовый сценарий "придти куда-то, выполнить задачу и (опционально) вернуться".
    /// Конкретные сценарии задают:
    /// 1) куда идти (GetTargetBuilding)
    /// 2) какую задачу выполнить (CreateMainTask)
    /// 3) куда возвращаться (GetReturnTarget, необязательно)
    /// </summary>
    public abstract class MoveAndPerformTaskScenario : ICitizenScenario
    {
        protected readonly IEntityMovementService _movement;
        protected readonly IBuildingRegistry _registry;

        protected MoveAndPerformTaskScenario(IEntityMovementService movement, IBuildingRegistry registry)
        {
            _movement = movement;
            _registry = registry;
        }

        public abstract bool CanRun(Citizen citizen, ISimulationTimeService time);

        /// <summary>
        /// Здание, куда нужно прийти.
        /// </summary>
        protected abstract Building GetTargetBuilding(Citizen citizen);

        /// <summary>
        /// Основная задача, которую нужно выполнить, когда гражданин прибыл.
        /// </summary>
        protected abstract ICitizenTask CreateMainTask(Citizen citizen, Building target);

        /// <summary>
        /// Куда возвращаться после выполнения задачи.
        /// По умолчанию — никуда.
        /// </summary>
        protected virtual Building? GetReturnTarget(Citizen citizen) => null;

        public virtual void BuildTasks(Citizen citizen)
        {
            var target = GetTargetBuilding(citizen);

            // 1. Идём к месту выполнения задачи
            citizen.Tasks.Enqueue(new MoveToBuildingTask(target, _movement, _registry));

            // 2. Выполняем основную задачу
            citizen.Tasks.Enqueue(CreateMainTask(citizen, target));

            // 3. (опционально) возвращаемся
            var returnTarget = GetReturnTarget(citizen);
            if (returnTarget != null)
            {
                citizen.Tasks.Enqueue(new MoveToBuildingTask(returnTarget, _movement, _registry));
            }
        }
    }
}
