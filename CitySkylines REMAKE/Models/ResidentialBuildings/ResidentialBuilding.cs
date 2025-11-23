using Core.Models.Base;

namespace CitySkylines_REMAKE.Models.ResidentialBuildings
{
    public class ResidentialBuilding : Building
    {
        public ResidentialBuildingType Type { get; }
        
        public ResidentialBuilding(ResidentialBuildingType type)
        {
            Type = type;
        }
    }
}
