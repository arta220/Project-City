using Domain.Common.Base;
using Domain.Map;

namespace Domain.Buildings.Infrastructure
{
    public class Airport : Building
    {
        public Airport(Area area)
            : base(
                floors: 1,
                maxOccupancy: 0,
                area: area)
        {
        }
    }
}