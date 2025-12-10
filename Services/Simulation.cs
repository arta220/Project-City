using Domain.Buildings.Construction;
using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Services.CitizensSimulation;
using Services.Common;
using Services.Construction;
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
        private readonly IConstructionService _constructionService;
        private readonly IConstructionMaterialLogisticsService _constructionLogisticsService;

        private readonly CitizenSimulationService _citizenSimulationService;
        private readonly TransportSimulationService _transportSimulationService;

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
            TransportSimulationService transportSimulationService,
            IUtilityService utilityService,
            IIndustrialProductionService productionService,
            IConstructionService constructionService,
            IConstructionMaterialLogisticsService constructionLogisticsService)
        {
            MapModel = mapModel;
            _placementService = placementService;
            _timeService = timeService;
            _placementRepository = placementRepository;
            _citizenSimulationService = citizenSimulationService;
            _transportSimulationService = transportSimulationService;
            _utilityService = utilityService;
            _productionService = productionService;
            _constructionService = constructionService;
            _constructionLogisticsService = constructionLogisticsService;

            _updatableServices.Add(citizenSimulationService);
            _updatableServices.Add(utilityService);
            _updatableServices.Add(transportSimulationService);
            _updatableServices.Add(productionService);
            _updatableServices.Add(constructionService);
            _updatableServices.Add(constructionLogisticsService);

            // Подписываемся на завершение строительства
            constructionService.ConstructionCompleted += OnConstructionCompleted;

            _timeService.TimeChanged += OnTimeChanged;
        }

        private void OnConstructionCompleted(ConstructionSite constructionSite)
        {
            CompleteConstruction(constructionSite);
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
                
                // Если размещается строительная площадка, запускаем строительство
                if (mapObject is ConstructionSite constructionSite)
                {
                    _constructionService.StartConstruction(constructionSite);
                    _constructionLogisticsService.RequestMaterialsDelivery(constructionSite);
                }
                
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

            // Если удаляется строительная площадка, отменяем строительство
            if (mapObject is ConstructionSite constructionSite)
            {
                _constructionService.CancelConstruction(constructionSite);
            }

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

        /// <summary>
        /// Обрабатывает завершение строительства - заменяет строительную площадку на построенное здание
        /// </summary>
        public void CompleteConstruction(ConstructionSite constructionSite)
        {
            if (constructionSite.Project == null || constructionSite.Status != ConstructionSiteStatus.Completed)
                return;

            var (placement, found) = _placementRepository.TryGetPlacement(constructionSite);
            if (!found || placement == null)
                return;

            // Удаляем строительную площадку
            if (TryRemove(constructionSite))
            {
                // Создаем и размещаем построенное здание
                var builtObject = constructionSite.Project.TargetBuildingFactory.Create();
                if (TryPlace(builtObject, placement.Value))
                {
                    // Здание успешно размещено
                }
            }
        }
    }
}