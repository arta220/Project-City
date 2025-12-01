using Domain.Buildings.EducationBuildings;
using Domain.Citizens;
using Domain.Common.Enums;
using Services.BuildingRegistry;

namespace Services.Citizens.Education
{
    public class FindEducationService : IFindEducationService
    {
        private readonly IBuildingRegistry _buildingRegistry;

        public FindEducationService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        public IEnumerable<EducationBuilding> FindEducation(Citizen citizen)
        {
            var requiredLevel = GetRequiredEducationLevel(citizen.Age);

            return _buildingRegistry
                .GetBuildings<EducationBuilding>()
                .Where(b => b.HasCapacity && b.Type == requiredLevel);
        }

        private EducationType GetRequiredEducationLevel(int age)
        {
            return age switch
            {
                >= 7 and < 15 => EducationType.School,
                >= 15 and < 18 => EducationType.College,
                >= 18 => EducationType.University,
                _ => EducationType.None
            };
        }
    }
}
