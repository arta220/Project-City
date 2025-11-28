using Domain.Map;
using Domain.Enums;

namespace Domain.Buildings
{
    public class GasStation : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.GasStation;

        public GasStation(Area area)
            : base(area, serviceTime: 5, maxQueue: 6, workerCount: 2)
        {
        }
    }
}