using Domain.Map;
using Domain.Enums;

namespace Domain.Buildings
{
    public class Restaurant : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.Restaurant;

        public Restaurant(Area area)
            : base(area, serviceTime: 20, maxQueue: 18, workerCount: 6)
        {
        }
    }
}