using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Base;
using Domain.Buildings;
using Domain.Buildings.EducationBuildings;
using Domain.Citizens.States;
using Domain.Enums;
using Domain.Map;

namespace Domain.Citizens
{
    /// <summary>
    /// Представляет жителя города, с базовыми характеристиками и текущим состоянием.
    /// </summary>
    public class Citizen : ObservableObject
    {
        /// <summary>
        /// Возраст жителя.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Уровень образования жителя.
        /// </summary>
        public EducationType EducationLevel { get; set; }
        public EducationBuilding StudyPlace { get; set; }

        /// <summary>
        /// Текущая работа жителя.
        /// </summary>
        public Job CurrentJob { get; set; }
        public Building WorkPlace { get; set; }

        /// <summary>
        /// Текущая позиция жителя на карте.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Текущее состояние жителя (например, идёт на работу, дома и т.д.).
        /// </summary>
        public CitizenState State { get; set; }

        /// <summary>
        /// Дом жителя.
        /// </summary>
        public ResidentialBuilding Home { get; set; }

        /// <summary>
        /// Позиция, к которой житель направляется в данный момент.
        /// </summary>
        public Position TargetPosition { get; set; }

        /// <summary>
        /// Текущий путь жителя как очередь позиций, по которым он будет перемещаться.
        /// </summary>
        public Queue<Position> CurrentPath { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр жителя.
        /// </summary>
        public Citizen()
        {
            CurrentPath = new Queue<Position>();
        }

        /// <summary>
        /// Здоровье жителя
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        /// Уровень счастья жителя.
        /// Можно использовать для изменения поведения или мотивации.
        /// </summary>
        public float Happiness { get; set; }

        /// <summary>
        /// Денежные средства жителя.
        /// </summary>
        public float Money { get; set; }
    }
}
