using CitySkylines_REMAKE.Models;
using CitySkylines_REMAKE.Services.MapGenerator;
using CitySkylines_REMAKE.Services.PlaceBuilding;
using CitySkylines_REMAKE.ViewModels;
using CitySkylines_REMAKE.Views;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<ConstructionValidator>();
            services.AddSingleton<IBuildingPlacementService, BuildingPlacementService>();
            services.AddSingleton<IMapGenerator, MapGenerator>();

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