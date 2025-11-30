using Domain.Citizens;
using Domain.Time;

namespace Services.Interfaces
{
    public interface IPopulationService
    {
        void ProcessDemography(List<Citizen> citizens, SimulationTime time,
            Action<Citizen> onCitizenBorn, Action<Citizen> onCitizenDied);
    }
}
