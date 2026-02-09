using Domain.Citizens.States;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public class Cafe : CommercialBuilding
    {
        public override CommercialType CommercialType => CommercialType.Cafe;

        public Cafe(Area area)
            : base(area, serviceTime: 12, maxQueue: 12, workerCount: 4)
        {
        }
    }
}