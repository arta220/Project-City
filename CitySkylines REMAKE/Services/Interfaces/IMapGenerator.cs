using CitySkylines_REMAKE.Models.Map;

namespace CitySkylines_REMAKE.Services.Interfaces
{
    public interface IMapGenerator
    {
        MapModel GenerateMap(int width, int height);
    }
}
