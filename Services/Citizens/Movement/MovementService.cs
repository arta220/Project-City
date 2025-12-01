using Domain.Citizens;
using Domain.Map;
using Services.PathFind;
using Domain.Common.Time;
using Services.CitizensSimulation;

namespace Services.Citizens.Movement
{
    /// <summary>
    /// Сервис, отвечающий за перемещение граждан по карте.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется внутри <see cref="CitizenController"/> для реализации перехода граждан между позициями (дом, работа, школа и т.д.).
    /// - Вызывается на каждом тике симуляции через <see cref="CitizenSimulationService"/>.
    /// 
    /// Возможные расширения:
    /// - Добавить разные типы движения (пешком, транспортом).
    /// - Реализовать обход препятствий с учётом текущих MapObjects.
    /// - Оптимизировать пересчёт пути только при изменении карты.
    /// </remarks>
    public class MovementService : ICitizenMovementService
    {
        private readonly IPathFinder _pathFinder;

        /// <summary>
        /// Инициализирует сервис перемещения с заданным алгоритмом поиска пути.
        /// </summary>
        /// <param name="pathfinder">Сервис поиска пути.</param>
        public MovementService(IPathFinder pathfinder)
        {
            _pathFinder = pathfinder;
        }

        /// <summary>
        /// Перемещает гражданина к указанной позиции на текущем тике симуляции.
        /// </summary>
        /// <param name="citizen">Гражданин, которого нужно переместить.</param>
        /// <param name="position">Целевая позиция.</param>
        /// <param name="tick">Номер текущего тика симуляции.</param>
        public void Move(Citizen citizen, Position position, SimulationTime time)
        {
            if (citizen.CurrentPath.Count == 0 || citizen.TargetPosition != position)
            {
                citizen.TargetPosition = position;
                _pathFinder.FindPath(citizen.Position, citizen.TargetPosition, citizen.CurrentPath);
            }

            if (citizen.CurrentPath.Count > 0)
            {
                citizen.Position = citizen.CurrentPath.Dequeue();
            }
        }
    }
}
