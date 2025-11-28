using Domain.Base;
using Domain.Map;

namespace Domain.Buildings
{
    public class ResidentialBuilding : Building
    {
        /// <summary>
        /// Менеджер коммунальных систем только для жилого дома
        /// </summary>
        public UtilityManager Utilities { get; }

        public ResidentialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
            Utilities = new UtilityManager();
        }
    }
}
