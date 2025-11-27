using Domain.Base;
using Services.Interfaces;
using Domain.Map;
using Services.PlaceBuilding;
using Services.CitizensSimulation;
using Services.BuildingRegistry;
using Services.PathFind;
using Domain.Citizens;
using Services.SimulationClock;
using System.Collections.ObjectModel;

namespace Services
{
    public class Simulation
    {
        private readonly IMapObjectPlacementService _mapObjectPlacementService;
        private readonly IMapGenerator _mapGenerator;
        private readonly ISimulationClock _clock;
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly PlacementRepository _placementRepository;
        public readonly CitizenSimulationService _citizenSimulationService;

        public event Action<int> TickOccurred;
        public event Action<Citizen> CitizenAdded;
        public event Action<Citizen> CitizenRemoved;
        public event Action<MapObject> MapObjectPlaced;
        public event Action<MapObject> MapObjectRemoved;

        public ObservableCollection<Citizen> Citizens { get; } = new ObservableCollection<Citizen>();
        public ObservableCollection<MapObject> MapObjects { get; } = new ObservableCollection<MapObject>();

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

            _citizenSimulationService.CitizenAdded += OnCitizenAdded;
            _citizenSimulationService.CitizenRemoved += OnCitizenRemoved;


            _clock.TickOccurred += OnTick;
            _clock.Start();
        }

        public void OnTick(int tick)
        {
            _citizenSimulationService.UpdateAll(tick);
            TickOccurred?.Invoke(tick);
        }

        public bool TryPlace(MapObject mapObject, Placement placement)
        {
            if (_mapObjectPlacementService.TryPlace(MapModel, mapObject, placement))
            {
                _placementRepository.Register(mapObject, placement);
                MapObjects.Add(mapObject);
                MapObjectPlaced?.Invoke(mapObject);
                return true;
            }
            return false;
        }

        public bool RemoveMapObject(MapObject mapObject)
        {
            //if (_placementRepository.Remove(mapObject))
            //{
            //    MapObjects.Remove(mapObject);
            //    MapObjectRemoved?.Invoke(mapObject);
            //    return true;
            //}
            return false;
            
        }

        public void AddCitizen(Citizen citizen)
        {
            _citizenSimulationService.AddCitizen(citizen);
        }

        public void RemoveCitizen(Citizen citizen)
        {
//            _citizenSimulationService.RemoveCitizen(citizen);
        }

        public Placement GetMapObjectPlacement(MapObject mapObject) => _placementRepository.GetPlacement(mapObject);

        public bool CanPlace(MapObject mapObject, Placement placement)
        {
            return _mapObjectPlacementService.CanPlace(MapModel, mapObject, placement);
        }

        private void OnCitizenAdded(Citizen citizen)
        {
            Citizens.Add(citizen);
            CitizenAdded?.Invoke(citizen);
        }

        private void OnCitizenRemoved(Citizen citizen)
        {
            Citizens.Remove(citizen);
            CitizenRemoved?.Invoke(citizen);
        }
    }
}