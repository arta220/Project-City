using Domain.Base;
using Domain.Enums;
using Domain.Map;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Buildings
{
    public class JewelryFactory : IndustrialBuilding
    {
        public BuildingType BuildingType => BuildingType.JewelryFactory;

        public int TotalProduced { get; private set; }
        public decimal TotalRevenue { get; private set; }
        public bool IsActive { get; set; } = true;

        public JewelryFactory(Area area)
            : base(3, 50, area, "Jewelry")
        {
            TotalProduced = 0;
            TotalRevenue = 0;
        }

        public void AddProduction(int quantity, decimal revenue)
        {
            if (IsActive && quantity > 0)
            {
                TotalProduced += quantity;
                TotalRevenue += revenue;
            }
        }

        public void SetActive(bool active)
        {
            IsActive = active;
        }
    }
}