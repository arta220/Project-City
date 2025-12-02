using Domain.Buildings.EducationBuildings;
using Domain.Buildings.Residential;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;
using Domain.Transports.Ground;

namespace Domain.Citizens
{
    /// <summary>
    /// Представляет жителя города, с базовыми характеристиками и текущим состоянием.
    /// </summary>
    public class Citizen(Area area, float speed) : MovingEntity(area, speed)
    {
        public int Age { get; set; }
        public EducationType EducationLevel { get; set; }
        public EducationBuilding StudyPlace { get; set; }
        public CitizenProfession Profession { get; set; }
        public Building WorkPlace { get; set; }
        public Transport CurrentTransport { get; set; }
        public PersonalCar PersonalCar { get; set; }
        public bool HasCar => PersonalCar != null;
        public CitizenState State { get; set; }
        public ResidentialBuilding Home { get; set; }
        public float Health { get; set; }
        public float Happiness { get; set; }
        public float Money { get; set; }
    }
}
