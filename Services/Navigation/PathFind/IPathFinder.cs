using Domain.Map;

namespace Services.PathFind
{
    public interface IPathFinder
    {
        /// <summary>
        /// Поиск пути между двумя точками.
        /// </summary>
        /// <param name="current">Стартовая позиция.</param>
        /// <param name="target">Целевая позиция.</param>
        /// <param name="roadsOnly">
        /// Если true, путь должен проходить только по дорогам (для транспорта).
        /// Если false, используется обычная пешая навигация.
        /// </param>
        List<Position> FindPath(Position current, Position target, bool roadsOnly = false);
    }
}
