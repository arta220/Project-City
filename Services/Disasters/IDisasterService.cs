using Domain.Base;
using Domain.Common.Base;
using Domain.Common.Enums;
using Services.Common;

namespace Services.Disasters
{
    public interface IDisasterService : IUpdatable
    {
        void FixDisaster(Building building, DisasterType disasterType);
        Dictionary<DisasterType, int> GetActiveDisasters(Building building);
        Dictionary<Building, List<DisasterType>> GetDisasterMap();
    }
}