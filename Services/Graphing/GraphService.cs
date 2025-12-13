using System;
using System.Collections.Generic;
using System.Linq;
using Services.Utilities;
using Services.IndustrialProduction;

namespace Services.Graphing
{
    public class GraphService
    {
        private readonly List<IGraphDataProvider> _providers = new();
        private readonly IUtilityService _utilityService;
        private readonly IIndustrialProductionService? _productionService;

        public GraphService(IUtilityService utilityService, IIndustrialProductionService? productionService = null)
        {
            _utilityService = utilityService;
            _productionService = productionService;
            RegisterDefaultProviders();
        }

        public void RegisterProvider(IGraphDataProvider provider)
        {
            _providers.Add(provider);
        }

        private void RegisterDefaultProviders()
        {
            // Регистрируем графики для коммунальных услуг
            RegisterProvider(new UtilitiesGraphProvider(_utilityService));

            if (_productionService != null)
            {
                // Регистрируем новые графики для химического производства и логистики
                RegisterProvider(new ChemicalProductionGraphProvider(_productionService));
                RegisterProvider(new LogisticsGraphProvider(_productionService));
                RegisterProvider(new CombinedChemicalLogisticsGraphProvider(_productionService));
                RegisterProvider(new ProductionComparisonGraphProvider(_productionService));

                // Старые графики картона и упаковки (если нужны для обратной совместимости)
                // RegisterProvider(new CardboardProductionGraphProvider(_productionService));
                // RegisterProvider(new PackagingProductionGraphProvider(_productionService));
            }
        }

        public IEnumerable<IGraphDataProvider> GetAvailableGraphs()
        {
            return _providers;
        }

        public IGraphDataProvider? GetGraphProvider(string systemName)
        {
            return _providers.FirstOrDefault(p => p.SystemName == systemName);
        }
    }
}