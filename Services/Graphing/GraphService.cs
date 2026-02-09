using System;
using System.Collections.Generic;
using System.Linq;
using Services.CommercialVisits;
using Services.Utilities;
using Services.IndustrialProduction;

namespace Services.Graphing
{
    public class GraphService
    {
        private readonly List<IGraphDataProvider> _providers = new();
        private readonly IUtilityService _utilityService;
        private readonly IIndustrialProductionService? _productionService;
        private readonly ICommercialVisitService? _commercialVisitService;

        public GraphService(
            IUtilityService utilityService,
            IIndustrialProductionService? productionService = null,
            ICommercialVisitService? commercialVisitService = null)
        {
            _utilityService = utilityService;
            _productionService = productionService;
            _commercialVisitService = commercialVisitService;
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

            if (_commercialVisitService != null)
            {
                RegisterProvider(new CommercialVisitsGraphProvider(_commercialVisitService));
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
