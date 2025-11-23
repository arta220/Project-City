using CitySkylines_REMAKE.Models.Citizens;

namespace CitySkylines_REMAKE.Services.Interfaces
{
    public interface IPopulationService
    {
        void TryReproduce(Citizen mom, Citizen dad);
        void AgeCitizen(Citizen citizen);
    }
}
