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
using MaterialAvailabilityInfo = Services.Construction.MaterialAvailabilityInfo;
using Services.PlaceBuilding;
using Services.Time;
using Services.TransportSimulation;
using Services.Utilities;
using Domain.Factories;

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
        private readonly IConstructionProjectFactory _constructionProjectFactory;
        private readonly IConstructionMaterialAvailabilityService _materialAvailabilityService;

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
            IConstructionMaterialLogisticsService constructionLogisticsService,
            IConstructionProjectFactory constructionProjectFactory,
            IConstructionMaterialAvailabilityService materialAvailabilityService)
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
            _constructionProjectFactory = constructionProjectFactory;
            _materialAvailabilityService = materialAvailabilityService;

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

        /// <summary>
        /// Получает список сломанных коммунальных услуг в жилом здании
        /// </summary>
        /// <param name="building">Жилое здание для проверки</param>
        /// <returns>Словарь сломанных коммунальных услуг и их количество</returns>
        public Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building) => _utilityService.GetBrokenUtilities(building);

        /// <summary>
        /// Исправляет указанную коммунальную услугу в жилом здании
        /// </summary>
        /// <param name="building">Жилое здание</param>
        /// <param name="utilityType">Тип коммунальной услуги для исправления</param>
        public void FixBuildingUtility(ResidentialBuilding building, UtilityType utilityType) => _utilityService.FixUtility(building, utilityType);


        /// <summary>
        /// Размещает объект на карте. Если размещается здание (Building), автоматически создается строительная площадка
        /// </summary>
        /// <param name="mapObject">Объект для размещения на карте</param>
        /// <param name="placement">Позиция и размер объекта</param>
        /// <returns>True, если объект успешно размещен, иначе false</returns>
        public bool TryPlace(MapObject mapObject, Placement placement)
        {
            // Если размещается здание (но не строительная площадка), создаем строительную площадку
            if (mapObject is Building building && !(building is ConstructionSite))
            {
                // Для зданий нужно использовать TryPlaceFromFactory, но для обратной совместимости
                // размещаем напрямую, если это не стандартное здание
                if (_placementService.TryPlace(MapModel, building, placement))
                {
                    _placementRepository.Register(building, placement);
                    MapObjectPlaced?.Invoke(building);
                    return true;
                }
                return false;
            }

            // Для остальных объектов (дороги, парки и т.д.) размещаем напрямую
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

        /// <summary>
        /// Размещает объект на карте из фабрики. Если это здание, автоматически создается строительная площадка
        /// </summary>
        /// <param name="factory">Фабрика для создания объекта</param>
        /// <param name="placement">Позиция и размер объекта</param>
        /// <param name="materialAvailabilityInfo">Информация о доступности материалов (заполняется при размещении зданий)</param>
        /// <returns>True, если объект успешно размещен, иначе false</returns>
        public bool TryPlaceFromFactory(IMapObjectFactory factory, Placement placement, out MaterialAvailabilityInfo materialAvailabilityInfo)
        {
            materialAvailabilityInfo = null;

            if (factory == null)
                return false;

            var mapObject = factory.Create();
            
            // Если это промышленное здание (завод), размещаем напрямую без строительства
            if (mapObject is Domain.Buildings.IndustrialBuilding)
            {
                return TryPlace(mapObject, placement);
            }
            
            // Если это здание (Building), создаем строительную площадку
            if (mapObject is Building building && !(building is ConstructionSite))
            {
                return TryPlaceBuildingFromFactory(factory, building.Area, placement, out materialAvailabilityInfo);
            }

            // Для остальных объектов размещаем напрямую
            return TryPlace(mapObject, placement);
        }

        /// <summary>
        /// Размещает объект на карте из фабрики. Если это здание, автоматически создается строительная площадка
        /// </summary>
        /// <param name="factory">Фабрика для создания объекта</param>
        /// <param name="placement">Позиция и размер объекта</param>
        /// <returns>True, если объект успешно размещен, иначе false</returns>
        public bool TryPlaceFromFactory(IMapObjectFactory factory, Placement placement)
        {
            return TryPlaceFromFactory(factory, placement, out _);
        }

        /// <summary>
        /// Размещает здание через строительную площадку, используя фабрику
        /// </summary>
        /// <param name="buildingFactory">Фабрика здания для размещения</param>
        /// <param name="buildingArea">Площадь здания</param>
        /// <param name="placement">Позиция и размер здания</param>
        /// <param name="materialAvailabilityInfo">Информация о доступности материалов</param>
        /// <returns>True, если строительная площадка успешно создана, иначе false</returns>
        private bool TryPlaceBuildingFromFactory(IMapObjectFactory buildingFactory, Area buildingArea, Placement placement, out MaterialAvailabilityInfo materialAvailabilityInfo)
        {
            materialAvailabilityInfo = null;

            // Создаем проект строительства
            var project = _constructionProjectFactory.CreateProject(buildingFactory);
            if (project == null)
            {
                // Если проект не определен, создаем и размещаем здание напрямую
                var building = buildingFactory.Create();
                return TryPlace(building, placement);
            }

            // Проверяем доступность материалов
            materialAvailabilityInfo = _materialAvailabilityService.CheckMaterialAvailability(project);

            // Создаем строительную площадку (даже если материалов нет - пользователь может построить заводы позже)
            var constructionSite = new ConstructionSite(buildingArea, project);
            
            if (_placementService.TryPlace(MapModel, constructionSite, placement))
            {
                _placementRepository.Register(constructionSite, placement);
                _constructionService.StartConstruction(constructionSite);
                _constructionLogisticsService.RequestMaterialsDelivery(constructionSite);
                MapObjectPlaced?.Invoke(constructionSite);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Удаляет объект с карты. Если это строительная площадка, отменяет строительство
        /// </summary>
        /// <param name="mapObject">Объект для удаления с карты</param>
        /// <returns>True, если объект успешно удален, иначе false</returns>
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

        /// <summary>
        /// Получает позицию объекта на карте
        /// </summary>
        /// <param name="mapObject">Объект для получения позиции</param>
        /// <returns>Кортеж с позицией объекта и флагом успешного поиска</returns>
        public (Placement? placement, bool found) GetMapObjectPlacement(MapObject mapObject) => _placementRepository.TryGetPlacement(mapObject);

        /// <summary>
        /// Проверяет, можно ли разместить объект на указанной позиции
        /// </summary>
        /// <param name="mapObject">Объект для проверки</param>
        /// <param name="placement">Позиция и размер объекта</param>
        /// <returns>True, если объект можно разместить, иначе false</returns>
        public bool CanPlace(MapObject mapObject, Placement placement) => _placementService.CanPlace(MapModel, mapObject, placement);
        /// <summary>
        /// Добавляет жителя в симуляцию
        /// </summary>
        /// <param name="citizen">Житель для добавления</param>
        public void AddCitizen(Citizen citizen)
        {
            _citizenSimulationService.AddCitizen(citizen);
            _placementRepository.Register(citizen, new Placement(citizen.Position, citizen.Area));
        }

        /// <summary>
        /// Добавляет транспорт в симуляцию
        /// </summary>
        /// <param name="transport">Транспорт для добавления</param>
        public void AddTransport(Transport transport)
        {
            _transportSimulationService.AddTransport(transport);
            _placementRepository.Register(transport, new Placement(transport.Position, transport.Area));
        }

        /// <summary>
        /// Удаляет жителя из симуляции
        /// </summary>
        /// <param name="citizen">Житель для удаления</param>
        public void RemoveCitizen(Citizen citizen) => _citizenSimulationService.RemoveCitizen(citizen);

        /// <summary>
        /// Удаляет транспорт из симуляции
        /// </summary>
        /// <param name="car">Транспорт для удаления</param>
        public void RemoveTransport(Transport car) => _transportSimulationService.RemoveTransport(car);

        /// <summary>
        /// Обрабатывает завершение строительства - заменяет строительную площадку на построенное здание
        /// </summary>
        /// <param name="constructionSite">строительная площадка</param>
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
                    // Пока не уверен, нужна ли дополнительная логика здесь
                }
            }
        }
    }
}