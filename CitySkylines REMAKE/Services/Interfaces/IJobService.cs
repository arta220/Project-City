using CitySkylines_REMAKE.Models.Citizens;
using CitySkylines_REMAKE.Models.Citizens.States;

namespace CitySkylines_REMAKE.Services.Interfaces
{
    public interface IJobService
    {
        Job? FindJobFor(Citizen citizen);
        void UpdateWork(Citizen citizen); 
    }
}
