using Services.CitizensSimulation;
using Services.IndustrialProduction;
using Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Graphing
{
    public class GraphService
    {
        private readonly List<IGraphDataProvider> _providers = new();
        private readonly IUtilityService _utilityService;
        private readonly IIndustrialProductionService? _productionService;
        private readonly CitizenSimulationService _citizenSimulationService;

        public GraphService(IUtilityService utilityService,
            IIndustrialProductionService? productionService = null,
            CitizenSimulationService? citizenSimulationService = null)
        {
            _utilityService = utilityService;
            _productionService = productionService;
            _citizenSimulationService = citizenSimulationService;
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

            if (_citizenSimulationService != null)
            {
                RegisterProvider(new EmploymentGraphProvider(_citizenSimulationService));
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
