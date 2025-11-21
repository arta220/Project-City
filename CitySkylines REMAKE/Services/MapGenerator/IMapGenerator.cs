using CitySkylines_REMAKE.Models.Map;

namespace CitySkylines_REMAKE.Services.MapGenerator
{
    public interface IMapGenerator
    {
        MapModel GenerateMap(int width, int height);
    }
}
