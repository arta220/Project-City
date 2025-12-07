using Domain.Citizens;
using Domain.Common.Enums;
using Domain.Common.Time;

namespace Services.Citizens.Education
{
    public class EducationService : IEducationService
    {
        public void UpdateEducation(Citizen citizen, SimulationTime time)
        {
            if (citizen.StudyPlace == null)
                return;

            var newLevel = GetEducationLevelForAge(citizen.Age);

            if (citizen.EducationLevel < newLevel)
            {
                citizen.EducationLevel = newLevel;
            }
        }

        private EducationType GetEducationLevelForAge(int age)
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
