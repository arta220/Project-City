using Domain.Base;
using Domain.Common.Enums;
using Domain.Map;
using Services.BuildingRegistry;

namespace Services.NavigationMap
{
    /// <summary>
    /// Сервис навигации по карте.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется в алгоритмах поиска пути (<see cref="Services.PathFind.IPathFinder"/>).
    /// - Определяет стоимость перемещения по плитке и проверяет возможность пройти по ней.
    /// 
    /// Возможные расширения:
    /// - Добавление влияния зданий или пробок на стоимость движения.
    /// - Поддержка разных типов перемещения (пешком, автомобилем, транспортом).
    /// - Расширение условий проходимости для специальных объектов (мосты, двери и т.д.).
    /// </remarks>
    public class NavigationMapService : INavigationMap
    {
        private readonly MapModel _map;
        private readonly IBuildingRegistry _buildingRegistry;

        /// <summary>
        /// Инициализирует сервис навигации по карте.
        /// </summary>
        /// <param name="map">Модель карты <see cref="MapModel"/>.</param>
        /// <param name="buildingRegistry">Реестр зданий для проверки препятствий.</param>
        public NavigationMapService(MapModel map, IBuildingRegistry buildingRegistry)
        {
            _map = map;
            _buildingRegistry = buildingRegistry;
        }

        /// <summary>
        /// Получает стоимость перемещения через указанную плитку.
        /// </summary>
        /// <param name="p">Позиция плитки.</param>
        /// <returns>Целочисленная стоимость перемещения.</returns>
        public int GetTileCost(Position p)
        {
            var tile = _map[p];

            int baseCost = 10;

            if (tile.Terrain == TerrainType.Plain)
                baseCost = 15;
            else if (tile.Terrain == TerrainType.Mountain)
                baseCost = 30;

            return baseCost;
        }

        /// <summary>
        /// Проверяет, можно ли пройти по плитке.
        /// </summary>
        /// <param name="p">Текущая позиция.</param>
        /// <param name="goal">Целевая позиция.</param>
        /// <param name="roadsOnly">Если true, разрешаем движение только по дорогам.</param>
        /// <returns>True, если плитка проходима; иначе false.</returns>
        public bool IsWalkable(Position p, Position goal, bool roadsOnly = false)
        {
            if (!_map.IsPositionInBounds(p))
                return false;

            if (p.Equals(goal))
                return true;

            var tile = _map[p];

            if (roadsOnly)
            {
                // Для транспорта: можно ехать только по клеткам, где построена дорога (объект Road)
                if (tile.MapObject is Road)
                    return true;

                return false;
            }

            // Обычные правила для пешеходов
            if (tile.MapObject != null)
                return false;

            if (tile.Terrain == TerrainType.Water)
                return false;

            return true;
        }
    }
}
