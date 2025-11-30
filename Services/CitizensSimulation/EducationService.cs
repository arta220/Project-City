using Domain.Buildings.EducationBuildings;
using Domain.Citizens;
using Domain.Enums;
using Domain.Time;
using Services.BuildingRegistry;
using Services.Interfaces;

namespace Services.CitizensSimulation
{
    public class EducationService : IEducationService
    {
        private readonly IBuildingRegistry _buildingRegistry;

        public EducationService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        public void UpdateEducation(Citizen citizen, SimulationTime time)
        {
            if (ShouldChangeEducationLevel(citizen))
            {
                FindNewEducation(citizen);
                return;
            }
        }

        private bool ShouldChangeEducationLevel(Citizen citizen)
        {
            return (citizen.Age == 7 && citizen.EducationLevel < EducationType.School) ||
                   (citizen.Age == 15 && citizen.EducationLevel < EducationType.College) ||
                   (citizen.Age == 18 && citizen.EducationLevel < EducationType.University);
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

        private void FindNewEducation(Citizen citizen)
        {
            var educationLevel = GetRequiredEducationLevel(citizen.Age);
            var availableSchools = _buildingRegistry
                .GetBuildings<EducationBuilding>()
                .Where(building => building.HasCapacity && building.Type == educationLevel)
                .ToList();

            if (availableSchools.Any())
            {
                var selectedSchool = availableSchools.First();
                selectedSchool.AddStudent(citizen);

                citizen.StudyPlace.RemoveStudent(citizen);

                citizen.StudyPlace = selectedSchool;
            }
        }
    }
}
