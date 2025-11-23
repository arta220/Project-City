using CitySkylines_REMAKE.Models.Enums;
using CitySkylines_REMAKE.Models.Map;
using CitySkylines_REMAKE.Services.Interfaces;

namespace CitySkylines_REMAKE.Services.MapGenerator
{
    public class MapGenerator : IMapGenerator
    {
        public MapModel GenerateMap(int width, int height)
        {
            var map = new MapModel(width, height);

            GenerateTerrain(map);
            // генерация ресурсов, высоты и т.д. типа таво

            return map;
        }

        private void GenerateTerrain(MapModel map)
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    map[x, y] = new TileModel { Terrain = TerrainType.Plain };
                }
            }
        }
    }
}
