using CitySkylines_REMAKE.Models.Citizens.States;
using CitySkylines_REMAKE.Models.Map;
using CitySkylines_REMAKE.Models.ResidentialBuildings;

namespace CitySkylines_REMAKE.Models.Citizens
{
    public class Citizen
    {
        public int Age { get; set; }
        public EducationLevel Education { get; set; }
        public Job CurrentJob { get; set; }
        public Position Position { get; set; }
        public CitizenState State { get; set; } 

        public ResidentialBuilding Home { get; set; }

        public float Health { get; set; }
        public float Happiness { get; set; }
        public float Money { get; set; }
    }

}
