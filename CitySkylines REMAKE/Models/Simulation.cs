using CitySkylines_REMAKE.Models.Enums;
using CitySkylines_REMAKE.Models.Map;
using CitySkylines_REMAKE.Services.MapGenerator;

namespace CitySkylines_REMAKE.Models
{
    public class Simulation
    {
        private readonly IMapGenerator _mapGenerator;
        private const int DEFAULT_MAP_SIZE = 25;
        public MapModel MapModel { get; private set; }
        public Simulation(IMapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;
            InitializeSimulation();
        }
        
        private void InitializeSimulation()
        {
            MapModel = _mapGenerator.GenerateMap(DEFAULT_MAP_SIZE, DEFAULT_MAP_SIZE);
        }
    }
}
