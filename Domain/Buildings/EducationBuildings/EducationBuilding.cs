using Domain.Base;
using Domain.Citizens;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings.EducationBuildings
{
    public class EducationBuilding : Building
    {
        public EducationBuilding(int floors, int maxOccupancy, Area area) : base(floors, maxOccupancy, area) { }

        public EducationType Type { get; set; }
        public int Capacity { get; set; }
        public int CurrentStudents { get; set; }

        public bool HasCapacity => CurrentStudents < Capacity;

        public void AddStudent(Citizen citizen)
        {
            if (HasCapacity) CurrentStudents++;
        }

        public void RemoveStudent(Citizen citizen)
        {
            if (CurrentStudents > 0) CurrentStudents--;
        }
    }
}
