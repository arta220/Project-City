using Services.Interfaces;
using Services.CitizensSimulation;
using Services.MapGenerator;
using Services.PlaceBuilding;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using CitySimulatorWPF.Views;
using Services;
using CitySimulatorWPF.ViewModels;

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

            // Симуляция жителей
            services.AddSingleton<EducationService>();
            services.AddSingleton<JobService>();
            services.AddSingleton<MovementService>();
            services.AddSingleton<PopulationService>();
            services.AddSingleton<CitizenController>();
            services.AddSingleton<CitizenSimulationService>();

            // Размещение зданий
            services.AddSingleton<ConstructionValidator>();
            services.AddSingleton<IMapObjectPlacementService, MapObjectPlacementService>();

            services.AddSingleton<IMapGenerator, MapGenerator>();

            services.AddTransient<BuildingPanelViewModel>();
            services.AddTransient<MainVM>();
            services.AddTransient<MapVM>();

            services.AddTransient<Simulation>();

            services.AddSingleton<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}