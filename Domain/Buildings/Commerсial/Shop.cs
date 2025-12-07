using Domain.Map;
using Domain.Enums;

namespace Domain.Buildings
{
    public class Shop : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.Shop;

        public Shop(Area area)
            : base(area, serviceTime: 8, maxQueue: 9, workerCount: 3)
        {
        }
    }
}