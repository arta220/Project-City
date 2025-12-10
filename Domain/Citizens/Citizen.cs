using Domain.Buildings.EducationBuildings;
using Domain.Buildings.Residential;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Base.MovingEntities;
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
        private static int _nextId = 1;
        public readonly Position HomePosition;

        public int Id { get; } = _nextId++;
        public int Age { get; set; }
        public EducationType EducationLevel { get; set; }
        public EducationBuilding StudyPlace { get; set; }
        public CitizenProfession Profession { get; set; }
        public Building WorkPlace { get; set; }
        public Transport CurrentTransport { get; set; }
        public PersonalCar PersonalCar { get; set; }
        public bool HasCar => PersonalCar != null;

        public CitizenState State { get; set; }
        public Queue<ICitizenTask> Tasks { get; set; } = new();
        public ICitizenTask? CurrentTask { get; set; }

        public ResidentialBuilding Home { get; set; }
        public float Health { get; set; }
        public float Happiness { get; set; }
        public float Money { get; set; }
        /// <summary>
        /// Целевая остановка для выхода из общественного транспорта
        /// null - если гражданин не планирует выходить из транспорта
        /// </summary>
        public Position? TargetExitStop { get; set; }

        /// <summary>
        /// Проверяет, хочет ли гражданин выйти на указанной остановке
        /// </summary>
        /// <param name="stop">Позиция остановки</param>
        /// <returns>true, если гражданин хочет выйти на этой остановке</returns>
        public bool WantsToExitAt(Position stop)
        {
            return TargetExitStop != null && TargetExitStop.Equals(stop);
        }
    }
}