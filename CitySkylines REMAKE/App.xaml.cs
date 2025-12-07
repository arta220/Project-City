using CitySimulatorWPF.Services;
using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views;
using Domain.Map;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.BuildingRegistry;
using Services.Citizens.Education;
using Services.Citizens.Job;
using Services.Citizens.Movement;
using Services.Citizens.Population;
using Services.CitizensSimulation;
using Services.CitizensSimulation.CitizenSchedule;
using Services.Graphing;
using Services.IndustrialProduction;
using Services.Interfaces;
using Services.MapGenerator;
using Services.NavigationMap;
using Services.PathFind;
using Services.PlaceBuilding;
using Services.Time;
using Services.Time.Clock;
using Services.TransportSimulation;
using Services.TransportSimulation.StateHandlers;
using Services.Utilities;
using System.Windows;
using Services.JewelryProduction;
namespace CitySkylines_REMAKE
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }


        private void ConfigureServices(IServiceCollection services)
        {
            // Map и генератор
            services.AddSingleton<IMapGenerator, MapGenerator>();
            services.AddSingleton<PlacementRepository>();
            services.AddSingleton<IUtilityService, UtilityService>();
            services.AddSingleton<IIndustrialProductionService, IndustrialProductionService>();
            services.AddSingleton<GraphService>(sp =>
            {
                var utilityService = sp.GetRequiredService<IUtilityService>();
                var jewelryProductionService = sp.GetRequiredService<IJewelryProductionService>();
                var productionService = sp.GetRequiredService<IIndustrialProductionService>();
                return new GraphService(utilityService, jewelryProductionService, productionService);
            });
            services.AddTransient<ChartsWindowViewModel>();

            services.AddSingleton<MapModel>(sp =>
            {
                var generator = sp.GetRequiredService<IMapGenerator>();
                return generator.GenerateMap(50, 50);
            });

            // Реестр зданий
            services.AddSingleton<IBuildingRegistry, BuildingRegistryService>();
            services.AddSingleton<IJewelryProductionService, JewelryProductionService>();
            services.AddSingleton<JewelryProductionService>();

            // Навигация и PathFinding
            services.AddSingleton<INavigationMap>(sp =>
            {
                var map = sp.GetRequiredService<MapModel>();
                var registry = sp.GetRequiredService<IBuildingRegistry>();
                return new NavigationMapService(map, registry);
            });
            services.AddSingleton<IPathFinder, AStarPathFinder>();

            // Симуляция и часы
            services.AddSingleton<ISimulationClock, SimulationClock>();
            services.AddSingleton<ISimulationTimeService, SimulationTimeService>();

            // Размещение объектов на карте
            services.AddSingleton<ConstructionValidator>();
            services.AddSingleton<IMapObjectPlacementService, MapObjectPlacementService>();

            // Сервисы граждан
            services.AddSingleton<IFindEducationService, FindEducationService>();
            services.AddSingleton<IEducationService, EducationService>();
            services.AddSingleton<IFindJobService, FindJobService>();
            services.AddSingleton<ICitizenScheduler, CitizenScheduler>();
            services.AddSingleton<ICitizenMovementService, MovementService>();
            services.AddSingleton<IPopulationService, PopulationService>();

            services.AddSingleton<IJobBehaviour, UtilityWorkerBehaviour>();
            services.AddSingleton<ICitizenScheduler, CitizenScheduler>();
            services.AddSingleton<JobController>();


            // Контроллер граждан
            services.AddSingleton<CitizenController>(provider =>
            new CitizenController(
                provider.GetRequiredService<ICitizenMovementService>(),
                provider.GetRequiredService<IBuildingRegistry>(),
                provider.GetRequiredService<JobController>(),
                provider.GetRequiredService<IUtilityService>()));
            services.AddSingleton<ICitizenManagerService, CitizenManagerService>();
            services.AddSingleton<CitizenSimulationService>();

            // Транспорт
            services.AddSingleton<TransportMovementService>();

            // Регистрация обработчиков состояний транспорта
            services.AddSingleton<ITransportStateHandler, IdleAtHomeStateHandler>();
            services.AddSingleton<ITransportStateHandler, DrivingToWorkStateHandler>();
            services.AddSingleton<ITransportStateHandler, ParkedAtWorkStateHandler>();
            services.AddSingleton<ITransportStateHandler, DrivingHomeStateHandler>();

            services.AddSingleton<PersonalTransportController>(sp =>
            {
                var handlers = sp.GetRequiredService<IEnumerable<ITransportStateHandler>>();
                return new PersonalTransportController(handlers);
            });

            services.AddSingleton<TransportSimulationService>(sp =>
            {
                var controller = sp.GetRequiredService<PersonalTransportController>();
                var clock = sp.GetRequiredService<ISimulationClock>();
                return new TransportSimulationService(controller);
            });

            // Менеджер машин
            services.AddSingleton<ICarManagerService, CarManagerService>();

            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<MessageService>();

            // Сервисы работы с картой
            services.AddSingleton<IMapTileService, MapTileService>();
            services.AddSingleton<IRoadConstructionService>(sp =>
            {
                var tileService = sp.GetRequiredService<IMapTileService>();
                return new RoadConstructionService(tileService.Tiles);
            });
            services.AddSingleton<IPathConstructionService>(sp =>
            {
                var tileService = sp.GetRequiredService<IMapTileService>();
                return new PathConstructionService(tileService.Tiles);
            });

            services.AddSingleton<Simulation>(sp =>
            {
                var mapModel = sp.GetRequiredService<MapModel>();
                var placementService = sp.GetRequiredService<IMapObjectPlacementService>();
                var timeService = sp.GetRequiredService<ISimulationTimeService>();
                var placementRepository = sp.GetRequiredService<PlacementRepository>();
                var citizenSimulationService = sp.GetRequiredService<CitizenSimulationService>();
                var transportSimulationService = sp.GetRequiredService<TransportSimulationService>();
                var utilityService = sp.GetRequiredService<IUtilityService>();
                var productionService = sp.GetRequiredService<IIndustrialProductionService>();
                var jewelryProductionService = sp.GetRequiredService<IJewelryProductionService>();
                return new Simulation(
                    mapModel,
                    placementService,
                    timeService,
                    placementRepository,
                    citizenSimulationService,
                    transportSimulationService,
                    utilityService,
                    productionService,
                    jewelryProductionService);
            });

            // ViewModels
            services.AddTransient<HeaderPanelViewModel>();
            services.AddTransient<BuildingPanelViewModel>();
            services.AddTransient<MainVM>();
            services.AddTransient<MapVM>();

            // MainWindow
            services.AddSingleton<MainWindow>();
        }



        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}