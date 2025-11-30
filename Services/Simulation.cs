using Domain.Base;
using Domain.Buildings;
using Domain.Enums;
using Domain.Map;
using Domain.Time;
using Services.CitizensSimulation;
using Services.Interfaces;
using Services.PlaceBuilding;
using Services.Time;

namespace Services
{
    /// <summary>
    /// Центральный сервис симуляции города. Отвечает за:
    /// - хранение карты (<see cref="MapModel"/>),
    /// - размещение объектов на карте,
    /// - уведомление о тиках симуляции и изменениях объектов.
    /// </summary>
    public class Simulation
    {
        private readonly IMapObjectPlacementService _placementService;
        private readonly ISimulationTimeService _timeService;
        private readonly PlacementRepository _placementRepository;
        private readonly IUtilityService _utilityService;

        private readonly List<IUpdatable> _updatableServices = new();

        public event Action<SimulationTime> TimeChanged;

        public event Action<MapObject> MapObjectPlaced;

        public event Action<MapObject> MapObjectRemoved;

        public MapModel MapModel { get; private set; }

        public Simulation(
            MapModel mapModel,
            IMapObjectPlacementService placementService,
            ISimulationTimeService timeService,
            PlacementRepository placementRepository,
            CitizenSimulationService citizenSimulationService,
            IUtilityService utilityService)
        {
            MapModel = mapModel;
            _placementService = placementService;
            _timeService = timeService;
            _placementRepository = placementRepository;


            _updatableServices.Add(citizenSimulationService);
            _updatableServices.Add(utilityService);

            _timeService.TimeChanged += OnTimeChanged;
        }

        private void OnTimeChanged(SimulationTime time)
        {
            TimeChanged?.Invoke(time);

            foreach (var service in _updatableServices)
                service.Update(time);
        }

        public Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building) => _utilityService.GetBrokenUtilities(building);

        public void FixBuildingUtility(ResidentialBuilding building, UtilityType utilityType) => _utilityService.FixUtility(building, utilityType);


        public bool TryPlace(MapObject mapObject, Placement placement)
        {
            if (_placementService.TryPlace(MapModel, mapObject, placement))
            {
                _placementRepository.Register(mapObject, placement);
                MapObjectPlaced?.Invoke(mapObject);
                return true;
            }
            return false;
        }

        public bool TryRemove(MapObject mapObject)
        {
            var (placement, found) = _placementRepository.TryGetPlacement(mapObject);

            if (!found || placement is null)
                return false;

            return _placementService.TryRemove(MapModel, (Placement)placement);
        }

        public (Placement? placement, bool found) GetMapObjectPlacement(MapObject mapObject) => _placementRepository.TryGetPlacement(mapObject);

        public bool CanPlace(MapObject mapObject, Placement placement) => _placementService.CanPlace(MapModel, mapObject, placement);
    }
}