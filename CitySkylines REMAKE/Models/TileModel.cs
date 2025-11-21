using CitySkylines_REMAKE.Models.Enums;
using System.Windows.Data;

namespace CitySkylines_REMAKE.Models
{
    public class TileModel
    {
        public TerrainType Terrain { get; set; }
        // СЮДА TERRAIN, RESOURCES, BUILDINGS, HEIGHT И ПРОЧЕЕ ЧТО ДАНО В ЗАДАНИЯХ

        public TileModel(TerrainType terrainType)
        {
            Terrain = TerrainType.Plain;
        }
    }
}
