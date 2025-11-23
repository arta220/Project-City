using Domain.Base;
using Domain.Enums;

namespace Domain.Map
{
    public class TileModel
    {
        public TerrainType Terrain { get; set; }
        public Building Building { get; set; }
        public float Height { get; set; }

        public bool CanPlace(Building building)
        {
            if (Building != null)
                return false;

            if (Terrain != TerrainType.Plain)
                return false;

            return true;
        }
    }
}
