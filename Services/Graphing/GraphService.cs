using Services.Interfaces;
using Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.JewelryProduction;
using Services.IndustrialProduction;
using Services.GlassProduction;

namespace Services.Graphing
{
    public class GraphService
    {
        private readonly IJewelryProductionService _jewelryProductionService;
        private readonly IGlassProductionService _glassProductionService;
        private readonly List<IGraphDataProvider> _providers = new();
        private readonly IUtilityService _utilityService;
        private readonly IIndustrialProductionService? _productionService;
        private readonly Simulation? _simulation;

        public GraphService(IUtilityService utilityService, IJewelryProductionService jewelryProductionService, IGlassProductionService glassProductionService, IIndustrialProductionService? productionService = null, Simulation? simulation = null)
        {
            _utilityService = utilityService;
            _jewelryProductionService = jewelryProductionService;
            _glassProductionService = glassProductionService;
            _productionService = productionService;
            _simulation = simulation;
            RegisterDefaultProviders();
        }

        public void RegisterProvider(IGraphDataProvider provider)
        {
            _providers.Add(provider);
        }

        private void RegisterDefaultProviders()
        {
            RegisterProvider(new UtilitiesGraphProvider(_utilityService));
            RegisterProvider(new JewelryGraphProvider(_jewelryProductionService));
            RegisterProvider(new GlassGraphProvider(_glassProductionService));
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
