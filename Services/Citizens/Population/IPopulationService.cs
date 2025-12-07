using Domain.Citizens;
using Domain.Common.Time;

namespace Services.Citizens.Population
{
    public interface IPopulationService
    {
        void ProcessDemography(List<Citizen> citizens, SimulationTime time,
            Action<Citizen> onCitizenBorn, Action<Citizen> onCitizenDied);
    }
}
