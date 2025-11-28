using CitySimulatorWPF.Services;
using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views;
using CitySkylines_REMAKE.ViewModels;
using Domain.Map;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.BuildingRegistry;
using Services.CitizensSimulation;
using Services.Interfaces;
using Services.MapGenerator;
using Services.NavigationMap;
using Services.PathFind;
using Services.PlaceBuilding;
using Services.SimulationClock;
using System.Windows;
using System.Windows.Controls;

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
            services.AddSingleton<Services.Interfaces.IUtilityService, Services.Utilities.UtilityService>();

            services.AddSingleton<MapModel>(sp =>
            {
                var generator = sp.GetRequiredService<IMapGenerator>();
                return generator.GenerateMap(50, 50); // размер карты по умолчанию
            });

            // Реестр зданий
            services.AddSingleton<IBuildingRegistry, BuildingRegistryService>();

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
            services.AddSingleton<Simulation>();

            // Размещение объектов на карте
            services.AddSingleton<ConstructionValidator>();
            services.AddSingleton<IMapObjectPlacementService, MapObjectPlacementService>();

            // Сервисы граждан через интерфейсы
            services.AddSingleton<IEducationService, EducationService>();
            services.AddSingleton<IJobService, JobService>();
            services.AddSingleton<IPopulationService, PopulationService>();
            services.AddSingleton<ICitizenMovementService, MovementService>();

            // Контроллер и менеджеры граждан
            services.AddSingleton<CitizenController>();
            services.AddSingleton<ICitizenManagerService, CitizenManagerService>();
            services.AddSingleton<CitizenSimulationService>();

            // Сервисы работы с картой
            services.AddSingleton<IMapTileService, MapTileService>();
            services.AddSingleton<IRoadConstructionService, RoadConstructionService>(sp =>
            {
                var tileService = sp.GetRequiredService<IMapTileService>();
                return new RoadConstructionService(tileService.Tiles);
            });
            services.AddSingleton<MessageService, MessageService>();

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