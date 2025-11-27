using Domain.Map;
using Domain.Enums;
using Services.Interfaces;

namespace Services.MapGenerator
{
    /// <summary>
    /// Генератор карты для симуляции города.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Вызывается при инициализации <see cref="MapModel"/> в <see cref="Simulation"/>.
    /// - Создает сетку плиток (<see cref="TileModel"/>) с базовой топографией.
    /// 
    /// Возможные расширения:
    /// - Добавление рек, дорог, лесов и других типов местности.
    /// - Генерация ресурсов или специальных объектов на карте.
    /// - Поддержка разных алгоритмов генерации (шум Перлина, случайные зоны и т.д.).
    /// </remarks>
    public class MapGenerator : IMapGenerator
    {
        /// <summary>
        /// Генерирует новую карту указанного размера.
        /// </summary>
        /// <param name="width">Ширина карты.</param>
        /// <param name="height">Высота карты.</param>
        /// <returns>Сгенерированная карта <see cref="MapModel"/>.</returns>
        public MapModel GenerateMap(int width, int height)
        {
            var map = new MapModel(width, height);

            GenerateTerrain(map);

            return map;
        }

        /// <summary>
        /// Инициализирует плитки карты с базовой топографией.
        /// </summary>
        /// <param name="map">Карта для генерации плиток.</param>
        private void GenerateTerrain(MapModel map)
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    map[x, y] = new TileModel
                    {
                        Terrain = TerrainType.Plain,
                        Position = new Position(x, y)
                    };
                }
            }
        }
    }
}
