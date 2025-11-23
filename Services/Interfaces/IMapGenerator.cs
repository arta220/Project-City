using Domain.Map;

namespace Services.Interfaces
{
    public interface IMapGenerator
    {
        MapModel GenerateMap(int width, int height);
    }
}
