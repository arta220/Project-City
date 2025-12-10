using CitySimulatorWPF.Services;
using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views;
using CitySkylines_REMAKE.ViewModels;
using Domain.Common.Base.MovingEntities;
using Domain.Map;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.BuildingRegistry;
using Services.Citizens.Education;
using Services.Citizens.Job;
using Services.Citizens.Population;
using Services.Citizens.Scenaries;
using Services.Citizens.Scenarios;
using Services.CitizensSimulatiom;
using Services.CitizensSimulation;
using Services.CitizensSimulation.CitizenSchedule;
using Services.EntityMovement.PathFind;
using Services.EntityMovement.Profile;
using Services.EntityMovement.Service;
using Services.Factories;
using Services.Graphing;
using Services.IndustrialProduction;
using Services.Interfaces;
using Services.MapGenerator;
using Services.PathFind;
using Services.PlaceBuilding;
using Services.Time;
using Services.Time.Clock;
using Services.TransportSimulation;
using Services.TransportSimulation.StateHandlers;
using Services.Utilities;
using System.Windows;

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
                var productionService = sp.GetRequiredService<IIndustrialProductionService>();
                var employmentService = sp.GetRequiredService<CitizenSimulationService>();
                return new GraphService(utilityService, productionService, employmentService);
            });
            services.AddTransient<ChartsWindowViewModel>();

            services.AddSingleton<MapModel>(sp =>
            {
                var generator = sp.GetRequiredService<IMapGenerator>();
                return generator.GenerateMap(50, 50);
            });

            // Реестр зданий
            services.AddSingleton<IBuildingRegistry, BuildingRegistryService>();

            // Навигация и PathFinding
            services.AddSingleton<INavigationProfile, CitizenProfile>();
            services.AddSingleton<IPathFinder, AStarPathFinder>();
            services.AddSingleton<IEntityMovementService, EntityMovementService>();
            services.AddSingleton<INavigationProfile, CitizenProfile>();
            services.AddSingleton<CitizenFactory>();



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
            services.AddSingleton<IEntityMovementService, EntityMovementService>();
            services.AddSingleton<IPopulationService, PopulationService>();

            services.AddSingleton<ICitizenScheduler, CitizenScheduler>();

            // Сценарии поведения жителей
            services.AddSingleton<ICitizenScenario, HomeScenario>();
            services.AddScoped<ICitizenScenario, JobSearchScenario>();
            services.AddScoped<ICitizenScenario, UtilityWorkerScenario>();

            // Контроллер граждан
            services.AddSingleton<CitizenController>();
            services.AddSingleton<ICitizenManagerService, CitizenManagerService>();
            services.AddSingleton<CitizenSimulationService>();

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

            services.AddSingleton<MessageService, MessageService>();

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

            services.AddSingleton<Simulation>();

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