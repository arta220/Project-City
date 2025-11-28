using System.ComponentModel;
using Domain.Base;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
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