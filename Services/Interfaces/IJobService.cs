using Domain.Citizens;
using Domain.Citizens.States;

namespace Services.Interfaces
{
    public interface IJobService
    {
        Job? FindJobFor(Citizen citizen);
        void UpdateWork(Citizen citizen, int tick); 
    }
}
