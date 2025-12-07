using Services.Interfaces;
using Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.JewelryProduction;
using Services.IndustrialProduction;

namespace Services.Graphing
{
    public class GraphService
    {
        private readonly IJewelryProductionService _jewelryProductionService;
        private readonly List<IGraphDataProvider> _providers = new();
        private readonly IUtilityService _utilityService;
        private readonly IIndustrialProductionService? _productionService;
        private readonly Simulation? _simulation;

        public GraphService(IUtilityService utilityService, IJewelryProductionService jewelryProductionService, IIndustrialProductionService? productionService = null, Simulation? simulation = null)
        {
            _utilityService = utilityService;
            _jewelryProductionService = jewelryProductionService;
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
