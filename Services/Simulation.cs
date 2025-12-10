using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Services.CitizensSimulation;
using Services.Common;
using Services.Finance;
using Services.IndustrialProduction;
using Services.Interfaces;
using Services.PlaceBuilding;
using Services.Time;
using Services.TransportSimulation;
using Services.Utilities;

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
        private readonly IIndustrialProductionService _productionService;

        private readonly CitizenSimulationService _citizenSimulationService;
        private readonly TransportSimulationService _transportSimulationService;
        private readonly ICityMaintenanceService _cityMaintenanceService;

        private readonly List<IUpdatable> _updatableServices = new();

        public event Action<SimulationTime> TimeChanged;

        public event Action<MapObject> MapObjectPlaced;

        public event Action<MapObject> MapObjectRemoved;

        public MapModel MapModel { get; private set; }

        public IFinanceService FinanceService { get; }

        public Simulation(
        MapModel mapModel,
        IMapObjectPlacementService placementService,
        ISimulationTimeService timeService,
        PlacementRepository placementRepository,
        CitizenSimulationService citizenSimulationService,
        TransportSimulationService transportSimulationService,
        IUtilityService utilityService,
                    IIndustrialProductionService productionService,
        IFinanceService financeService,
        ICityMaintenanceService cityMaintenanceService)
        {
            MapModel = mapModel;
            _placementService = placementService;
            _timeService = timeService;
            _placementRepository = placementRepository;
            _citizenSimulationService = citizenSimulationService;
            _transportSimulationService = transportSimulationService;
            FinanceService = financeService;

            _cityMaintenanceService = cityMaintenanceService;
            _utilityService = utilityService;
            _productionService = productionService;

            _updatableServices.Add(citizenSimulationService);
            _updatableServices.Add(utilityService);
            _updatableServices.Add(transportSimulationService);
            _updatableServices.Add(_cityMaintenanceService);
            if (financeService is IUpdatable updatableFinance)
            {
                _updatableServices.Add(updatableFinance);
            }
            _updatableServices.Add(productionService);

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

            if (_placementService.TryRemove(MapModel, (Placement)placement))
            {
                MapObjectRemoved?.Invoke(mapObject);
                return true;
            }

            return false;
        }

        public (Placement? placement, bool found) GetMapObjectPlacement(MapObject mapObject) => _placementRepository.TryGetPlacement(mapObject);

        public bool CanPlace(MapObject mapObject, Placement placement) => _placementService.CanPlace(MapModel, mapObject, placement);
        public void AddCitizen(Citizen citizen)
        {
            _citizenSimulationService.AddCitizen(citizen);
            _placementRepository.Register(citizen, new Placement(citizen.Position, citizen.Area));
        }

        public void AddTransport(Transport transport)
        {
            _transportSimulationService.AddTransport(transport);
            _placementRepository.Register(transport, new Placement(transport.Position, transport.Area));
        }

        public void RemoveCitizen(Citizen citizen) => _citizenSimulationService.RemoveCitizen(citizen);
        public void RemoveTransport(Transport car) => _transportSimulationService.RemoveTransport(car);
    }
}