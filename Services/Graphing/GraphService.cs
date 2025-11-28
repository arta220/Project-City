using Services.Interfaces;
using Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.JewelryProduction;

namespace Services.Graphing
{
    public class GraphService
    {
        private readonly JewelryProductionService _jewelryProductionService;
        private readonly List<IGraphDataProvider> _providers = new();
        private readonly IUtilityService _utilityService;

        public GraphService(IUtilityService utilityService, JewelryProductionService jewelryProductionService)
        {
            _utilityService = utilityService;
            _jewelryProductionService = jewelryProductionService; 
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
