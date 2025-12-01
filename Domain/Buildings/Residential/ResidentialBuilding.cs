using System.ComponentModel;
using Domain.Buildings.Utility;
using Domain.Common.Base;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings.Residential
{
    public class ResidentialBuilding : Building
    {
        /// <summary>

        /// </summary>
        public UtilityManager Utilities { get; }

        public ResidentialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
            Utilities = new UtilityManager();
        }
    }
}