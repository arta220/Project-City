using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;

namespace Services.CitizensSimulation.StateHandlers
{
    public interface ICitizenStateHandler
    {
        bool CanHandle(CitizenState state);
        void Update(Citizen citizen, SimulationTime time);
    }
}
