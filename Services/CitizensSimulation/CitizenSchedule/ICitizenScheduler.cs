using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;

namespace Services.CitizensSimulation.CitizenSchedule
{
    public interface ICitizenScheduler 
    {
        CitizenState Decide(Citizen citizen, CitizenState current, SimulationTime time);
    }
}
