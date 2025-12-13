using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Utilities;
using Services.IndustrialProduction;
using Services.Disasters;

namespace Services.Graphing
{
    public class GraphService
    {
        private readonly List<IGraphDataProvider> _providers = new();
        private readonly IUtilityService _utilityService;
        private readonly IIndustrialProductionService? _productionService;
        private readonly IDisasterService? _disasterService;

        public GraphService(IUtilityService utilityService, IIndustrialProductionService? productionService = null, IDisasterService? disasterService = null)
        {
            _utilityService = utilityService;
            _productionService = productionService;
            _disasterService = disasterService;
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
                RegisterProvider(new CosmeticsProductionGraphProvider(_productionService));
                RegisterProvider(new AlcoholProductionGraphProvider(_productionService));
            }

            if (_disasterService != null)
            {
                RegisterProvider(new DisasterGraphProvider(_disasterService));
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