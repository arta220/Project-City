using Domain.Citizens;

namespace Services.CitizensSimulation.CitizenSchedule
{
    public interface ICitizenScheduler
    {
        void UpdateSchedule(Citizen citizen);
    }
}