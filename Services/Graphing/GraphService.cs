using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            RegisterProvider(new UtilitiesGraphProvider(_utilityService));
            
            if (_productionService != null)
            {
                RegisterProvider(new CardboardProductionGraphProvider(_productionService));
                RegisterProvider(new PackagingProductionGraphProvider(_productionService));
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
