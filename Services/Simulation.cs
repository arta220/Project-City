using Domain.Base;
using Services.Interfaces;
using Domain.Map;
using Services.PlaceBuilding;
using Services.SimulationClock;
using Services.CitizensSimulation;

namespace Services
{
    public class Simulation
    {
        private readonly IMapObjectPlacementService _mapObjectPlacementService;
        private readonly IMapGenerator _mapGenerator;
        private readonly ISimulationClock _clock;

        private readonly PlacementRepository _placementRepository = new();
        private readonly CitizenSimulationService _citizenSimulationService;

        private const int DEFAULT_MAP_SIZE = 75;
        public MapModel MapModel { get; private set; }
        public Simulation(
            IMapGenerator mapGenerator, 
            IMapObjectPlacementService buildingPlacementService,
            ISimulationClock clock)
        {
            _clock = clock;
            _mapGenerator = mapGenerator;
            _mapObjectPlacementService = buildingPlacementService;
            InitializeSimulation();

            _citizenSimulationService = new CitizenSimulationService(new CitizenController(), _clock); // как будто заменить потом на инжект 
            _clock.TickOccurred += OnTick;

            _clock.Start();
        }
        
        public void OnTick(int tick)
        {
            _citizenSimulationService.UpdateAll(tick);
        }
        private void InitializeSimulation()
        {
            MapModel = _mapGenerator.GenerateMap(DEFAULT_MAP_SIZE, DEFAULT_MAP_SIZE);
        }

        public bool TryPlace(MapObject mapObject, Placement placement)
        {
            if (_mapObjectPlacementService.TryPlace(MapModel, mapObject, placement))
            {
                _placementRepository.Register(mapObject, placement);
                return true;
            }
            return false;
        }

        public Placement GetMapObjectPlacement(MapObject mapObject) => _placementRepository.GetPlacement(mapObject);
        public bool CanPlace(MapObject mapObject, Placement placement)
        {
            return _mapObjectPlacementService.CanPlace(MapModel, mapObject, placement);
        }
    }
}
