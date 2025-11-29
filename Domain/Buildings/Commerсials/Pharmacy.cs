using Domain.Map;
using Domain.Enums;

namespace Domain.Buildings
{
    public class Pharmacy : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.Pharmacy;

        public Pharmacy(Area area)
            : base(area, serviceTime: 10, maxQueue: 6, workerCount: 2)
        {
        }
    }
}