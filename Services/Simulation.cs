using Domain.Base;
using Services.Interfaces;
using Domain.Map;
using Services.PlaceBuilding;
using Services.SimulationClock;
using Services.CitizensSimulation;
using Services.BuildingRegistry;
using Services.PathFind;
using Domain.Citizens;

namespace Services
{
    public class Simulation
    {
        private readonly IMapObjectPlacementService _mapObjectPlacementService;
        private readonly IMapGenerator _mapGenerator;
        private readonly ISimulationClock _clock;
        private readonly IBuildingRegistry _buildingRegistry;

        private readonly PlacementRepository _placementRepository;
        private readonly CitizenSimulationService _citizenSimulationService;

        public MapModel MapModel { get; private set; }
        public Simulation(
            IMapGenerator mapGenerator, 
            IMapObjectPlacementService buildingPlacementService,
            ISimulationClock clock, 
            IPathFinder pathfinder,
            PlacementRepository placementRepository,
            MapModel mapModel)
        {
            MapModel = mapModel;
            _placementRepository = placementRepository;
            _buildingRegistry = new BuildingRegistryService(_placementRepository);
            _clock = clock;
            _mapGenerator = mapGenerator;
            _mapObjectPlacementService = buildingPlacementService;


            _citizenSimulationService = new CitizenSimulationService(
                new CitizenController(
                    _buildingRegistry,
                    new MovementService(pathfinder),
                    new JobService(),
                    new EducationService(),
                    new PopulationService()), 
                    _clock); 

            _clock.TickOccurred += OnTick;

            _clock.Start();
        }
        
        public void OnTick(int tick)
        {
            _citizenSimulationService.UpdateAll(tick);


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

        public void AddCitizen(Citizen citizen) => _citizenSimulationService.AddCitizen(citizen);
        public Placement GetMapObjectPlacement(MapObject mapObject) => _placementRepository.GetPlacement(mapObject);
        public bool CanPlace(MapObject mapObject, Placement placement)
        {
            return _mapObjectPlacementService.CanPlace(MapModel, mapObject, placement);
        }
    }
}
