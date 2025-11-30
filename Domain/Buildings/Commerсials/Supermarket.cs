using Domain.Map;
using Domain.Enums;

namespace Domain.Buildings
{
    public class Supermarket : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.Supermarket;

        public Supermarket(Area area)
            : base(area, serviceTime: 15, maxQueue: 24, workerCount: 8)
        {
        }
    }
}
