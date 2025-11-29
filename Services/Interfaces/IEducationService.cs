using Domain.Citizens;
using Domain.Time;

namespace Services.Interfaces
{
    public interface IEducationService
    {
        void UpdateEducation(Citizen citizen, SimulationTime time);
    }
}
