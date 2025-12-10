using Domain.Buildings.EducationBuildings;
using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Services.BuildingRegistry;

namespace Services.Citizens.Tasks.Find
{
    public class FindEducationTask : ICitizenTask // Задача поиска учебного заведения
    {
        private readonly IBuildingRegistry _registry;
        private bool _isCompleted;

        public bool IsCompleted => _isCompleted;

        public FindEducationTask(IBuildingRegistry registry)
        {
            _registry = registry;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (_isCompleted) return;

            if (citizen.StudyPlace != null)
            {
                _isCompleted = true;
                return;
            }

            var requiredLevel = GetRequiredEducationLevel(citizen.Age);

            var school = _registry
                .GetBuildings<EducationBuilding>()
                .FirstOrDefault(b => b.Type == requiredLevel && b.HasCapacity);

            if (school != null)
                citizen.StudyPlace = school;

            _isCompleted = true;
        }

        private Domain.Common.Enums.EducationType GetRequiredEducationLevel(int age) => age switch
        {
            >= 7 and < 15 => Domain.Common.Enums.EducationType.School,
            >= 15 and < 18 => Domain.Common.Enums.EducationType.College,
            >= 18 => Domain.Common.Enums.EducationType.University,
            _ => Domain.Common.Enums.EducationType.None
        };
    }
}
