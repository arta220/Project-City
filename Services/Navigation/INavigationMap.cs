using Domain.Map;

namespace Services.NavigationMap
{
    public interface INavigationMap
    {
        /// <summary>
        /// Проверка, можно ли пройти по клетке.
        /// </summary>
        /// <param name="p">Клетка, которую проверяем.</param>
        /// <param name="goal">Цель пути (финишная клетка).</param>
        /// <param name="roadsOnly">
        /// Если true, считаем, что двигается транспорт и разрешаем ход только по дорогам.
        /// Если false, используем обычные правила для пешеходов.
        /// </param>
        bool IsWalkable(Position p, Position goal, bool roadsOnly = false);

        /// <summary>
        /// Стоимость прохода через клетку.
        /// </summary>
        int GetTileCost(Position position);
    }
}
