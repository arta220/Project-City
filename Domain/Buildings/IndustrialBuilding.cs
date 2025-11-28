using Domain.Base;
using Domain.Map;

namespace Domain.Buildings
{
    public class IndustrialBuilding : Building
    {
        public string IndustryType { get; set; }
        public decimal Revenue { get; protected set; }
        public int ProductionCapacity { get; set; }

        public IndustrialBuilding(int floors, int maxOccupancy, Area area, string industryType = "General")
            : base(floors, maxOccupancy, area)
        {
            IndustryType = industryType;
            ProductionCapacity = 10;
            Revenue = 0;
        }

        public virtual void AddRevenue(decimal amount)
        {
            Revenue += amount;
        }
    }
}