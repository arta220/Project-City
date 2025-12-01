using Domain.Buildings.EducationBuildings;
using Domain.Citizens;

namespace Services.Citizens.Education
{
    public interface IFindEducationService 
    {
        IEnumerable<EducationBuilding> FindEducation(Citizen citizen);
    }
}
