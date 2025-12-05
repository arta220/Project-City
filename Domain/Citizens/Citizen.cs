using Domain.Buildings.EducationBuildings;
using Domain.Buildings.Residential;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
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

        /// <summary>
        /// Текущий транспорт, в котором находится житель (если он едет).
        /// </summary>
        public Transport CurrentTransport { get; set; }

        /// <summary>
        /// Личная машина жителя. Если null, житель ходит только пешком.
        /// </summary>
        public PersonalCar PersonalCar { get; set; }

        /// <summary>
        /// Простой флаг, есть ли у жителя машина.
        /// </summary>
        public bool HasCar => PersonalCar != null;

        /// <summary>
        /// Житель находится внутри транспорта (едет), если CurrentTransport не равен null.
        /// </summary>
        public bool IsInTransport => CurrentTransport != null;

        public CitizenState State { get; set; }
        public Queue<CitizenTask> Tasks { get; set; } = new();
        public CitizenTask? CurrentTask { get; set; }

        public ResidentialBuilding Home { get; set; }
        public float Health { get; set; }
        public float Happiness { get; set; }
        public float Money { get; set; }
    }
}
