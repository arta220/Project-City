using System;
using System.Collections.Generic;
using System.Linq;
using Services.Utilities;
using Services.Statistic;


namespace Services.Graphing
{
    /// <summary>
    /// Центральный сервис, который знает обо всех доступных графиках
    /// (ЖКХ, ресурсы и т.д.) и отдаёт их во ViewModel окна графиков.
    /// </summary>
    public class GraphService
    {
        private readonly List<IGraphDataProvider> _providers = new();

        private readonly IUtilityService _utilityService;
        private readonly IResourceStatisticsService _resourceStatisticsService;

        public GraphService(
            IUtilityService utilityService,
            IResourceStatisticsService resourceStatisticsService)
        {
            _utilityService = utilityService;
            _resourceStatisticsService = resourceStatisticsService;

            RegisterDefaultProviders();
        }

        /// <summary>
        /// Регистрирует внешний провайдер графиков (если захочешь добавлять ещё).
        /// </summary>
        public void RegisterProvider(IGraphDataProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _providers.Add(provider);
        }

        /// <summary>
        /// Регистрируем "стандартные" графики:
        /// - ЖКХ (UtilitiesGraphProvider)
        /// - Природные ресурсы (ResourcesGraphProvider)
        /// </summary>
        private void RegisterDefaultProviders()
        {
            // Графики по системе ЖКХ
            RegisterProvider(new UtilitiesGraphProvider(_utilityService));

            // Графики по природным ресурсам на карте
            RegisterProvider(new ResourcesGraphProvider(_resourceStatisticsService));
        }

        /// <summary>
        /// Все доступные графики для окна ChartsWindow.
        /// </summary>
        public IEnumerable<IGraphDataProvider> GetAvailableGraphs()
        {
            return _providers;
        }

        /// <summary>
        /// Найти провайдер по имени системы (SystemName).
        /// </summary>
        public IGraphDataProvider? GetGraphProvider(string systemName)
        {
            return _providers.FirstOrDefault(p => p.SystemName == systemName);
        }
    }
}
