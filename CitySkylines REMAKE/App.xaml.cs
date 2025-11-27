using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views;
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

            services.AddSingleton<INavigationMap>(sp =>
            {
                var map = sp.GetRequiredService<MapModel>();
                var registry = sp.GetRequiredService<IBuildingRegistry>();
                return new NavigationMapService(map, registry);
            });

            services.AddSingleton<IPathFinder, AStarPathFinder>();

            services.AddSingleton<ISimulationClock, SimulationClock>();

            // Размещение зданий
            services.AddSingleton<ConstructionValidator>();
            services.AddSingleton<IMapObjectPlacementService, MapObjectPlacementService>();

            // Сервисы граждан
            services.AddSingleton<EducationService>();
            services.AddSingleton<JobService>();
            services.AddSingleton<PopulationService>();
            services.AddSingleton<MovementService>();
            services.AddSingleton<CitizenController>();
            services.AddSingleton<CitizenSimulationService>();

            // ViewModels
            services.AddTransient<BuildingPanelViewModel>();
            services.AddTransient<MainVM>();
            services.AddTransient<MapVM>();

            // Симуляция
            services.AddSingleton<Simulation>();

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