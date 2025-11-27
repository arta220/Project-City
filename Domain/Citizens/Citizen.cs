using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Buildings;
using Domain.Citizens.States;
using Domain.Map;

namespace Domain.Citizens
{
    public class Citizen : ObservableObject
    {
        public int Age { get; set; }
        public EducationLevel Education { get; set; }
        public Job CurrentJob { get; set; }
        public Position Position { get; set; }
        public CitizenState State { get; set; } 

        public ResidentialBuilding Home { get; set; }

        public Position TargetPosition { get; set; }
        public Queue<Position> CurrentPath { get; set; }

        public Citizen()
        {
            CurrentPath = new();
        }

        public float Health { get; set; }
        public float Happiness { get; set; }
        public float Money { get; set; }
    }

}
