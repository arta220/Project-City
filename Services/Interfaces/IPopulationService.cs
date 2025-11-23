using Domain.Citizens;

namespace Services.Interfaces
{
    public interface IPopulationService
    {
        void TryReproduce(Citizen mom, Citizen dad);
        void AgeCitizen(Citizen citizen);
    }
}
