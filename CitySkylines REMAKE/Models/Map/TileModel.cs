using CitySkylines_REMAKE.Models.Enums;
using Core.Models.Base;
using System.Windows.Data;

namespace CitySkylines_REMAKE.Models.Map
{
    // класс со свойствами тайла
    public class TileModel
    {
        public TerrainType Terrain { get; set; }
        // СЮДА TERRAIN, RESOURCES, BUILDINGS, HEIGHT И ПРОЧЕЕ ЧТО ДАНО В ЗАДАНИЯХ
        public Building Building { get; set; }
    }
}
