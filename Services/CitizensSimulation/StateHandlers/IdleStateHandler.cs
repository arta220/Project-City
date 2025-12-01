using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Services.CitizensSimulation.CitizenSchedule;
namespace Services.CitizensSimulation.StateHandlers
{
    public class IdleStateHandler : ICitizenStateHandler
    {
        private readonly ICitizenScheduler _citizenScheduler;
        public IdleStateHandler(
            ICitizenScheduler citizenScheduler) 
        {
            _citizenScheduler = citizenScheduler;
        }
        public bool CanHandle(CitizenState state) => state == CitizenState.Idle;

        public void Update(Citizen citizen, SimulationTime time) 
            => citizen.State = _citizenScheduler.Decide(citizen, citizen.State, time);
    }
}
