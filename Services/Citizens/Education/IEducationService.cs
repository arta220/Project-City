using Domain.Citizens;
using Domain.Common.Time;

namespace Services.Citizens.Education
{
    public interface IEducationService
    {
        void UpdateEducation(Citizen citizen, SimulationTime time);
    }
}
