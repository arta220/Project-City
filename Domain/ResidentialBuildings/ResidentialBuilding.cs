using Domain.Base;

namespace Domain.ResidentialBuildings
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
