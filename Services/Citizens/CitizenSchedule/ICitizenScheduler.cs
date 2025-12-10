using Domain.Citizens;

namespace Services.Citizens.CitizenSchedule
{
    public interface ICitizenScheduler
    {
        void UpdateSchedule(Citizen citizen);
    }
}