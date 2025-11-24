using Domain.Base;
using Services.Interfaces;
using Domain.Map;

namespace Services
{
    public class Simulation
    {
        private readonly IBuildingPlacementService _buildingPlacementService;
        private readonly IMapGenerator _mapGenerator;
        private const int DEFAULT_MAP_SIZE = 25;
        public MapModel MapModel { get; private set; }
        public Simulation(IMapGenerator mapGenerator, IBuildingPlacementService buildingPlacementService)
        {
            _mapGenerator = mapGenerator;
            _buildingPlacementService = buildingPlacementService;
            InitializeSimulation();
        }
        
        private void InitializeSimulation()
        {
            MapModel = _mapGenerator.GenerateMap(DEFAULT_MAP_SIZE, DEFAULT_MAP_SIZE);
        }

        public bool TryPlaceBuilding(Building building, Area area)
        {
            return _buildingPlacementService.TryPlaceBuilding(MapModel, building, area);
        }

        public bool CanPlaceBuilding(Building building, Area area)
        {
            return _buildingPlacementService.CanPlaceBuilding(MapModel, building, area);
        }
    }
}
