using Domain.Citizens.States;
using Domain.Map;
using Domain.ResidentialBuildings;

namespace Domain.Citizens
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
