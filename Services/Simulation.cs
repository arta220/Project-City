using Domain.Base;
using Services.Interfaces;
using Domain.Map;
using Services.PlaceBuilding;
using Services.SimulationClock;
using Services.CitizensSimulation;
using Services.BuildingRegistry;
using Services.PathFind;
using Services.Utilities;
using Domain.Citizens;
using Domain.Enums;

namespace Services
{
    public class Simulation
    {
        private readonly IMapObjectPlacementService _mapObjectPlacementService;
        private readonly IMapGenerator _mapGenerator;
        private readonly ISimulationClock _clock;
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly IUtilityService _utilityService; // Smirnov

        private readonly PlacementRepository _placementRepository;
        private readonly CitizenSimulationService _citizenSimulationService;

        public MapModel MapModel { get; private set; }
        public Simulation(
            IMapGenerator mapGenerator, 
            IMapObjectPlacementService buildingPlacementService,
            ISimulationClock clock, 
            IPathFinder pathfinder,
            PlacementRepository placementRepository,
            MapModel mapModel,
            IUtilityService utilityService) // Smirnov
        {
            MapModel = mapModel;
            _placementRepository = placementRepository;
            _buildingRegistry = new BuildingRegistryService(_placementRepository);
            _clock = clock;
            _mapGenerator = mapGenerator;
            _mapObjectPlacementService = buildingPlacementService;
            _utilityService = utilityService; // Smirnov


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

            var buildings = _placementRepository.GetAllBuildings(); // Smirnov
            _utilityService.SimulateUtilitiesBreakdown(tick, buildings); // Sminov
        }

        // Smirnov
        public Dictionary<UtilityType, int> GetBrokenUtilities(Building building)
        {
            return _utilityService.GetBrokenUtilities(building);
        }

        // Smirnov
        public void FixBuildingUtility(Building building, UtilityType utilityType) 
        {
            _utilityService.FixUtility(building, utilityType);
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

        // Smirnov *
        public void RemoveBuilding(MapObject mapObject)
        {
            // Получаем размещение объекта
            var placement = _placementRepository.GetPlacement(mapObject);

            // Убираем объект со всех тайлов в области размещения
            for (int x = placement.Position.X; x < placement.Position.X + placement.Area.Width; x++)
            {
                for (int y = placement.Position.Y; y < placement.Position.Y + placement.Area.Height; y++)
                {
                    if (x < MapModel.Width && y < MapModel.Height &&
                        MapModel[x, y].MapObject == mapObject)
                    {
                        MapModel[x, y].MapObject = null;
                    }
                }
            }

            // Удаляем из репозитория
            _placementRepository.Remove(mapObject);
        }
    }
}
