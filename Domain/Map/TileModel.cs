using Domain.Base;
using Domain.Enums;

namespace Domain.Map
{
    public class TileModel
    {
        public Position Position { get; set; }
        public TerrainType Terrain { get; set; }
        public MapObject MapObject { get; set; }
        public float Height { get; set; }

        public bool CanPlace(MapObject mapObject)
        {
            return MapObject == null;
        }
    }
}
