using Domain.Citizens;
using Services.Citizens.Scenaries;
using Services.Time;

namespace Services.Citizens.Scenarios
{
    public class TransportSessionScenario : ICitizenScenario
    {
        public void BuildTasks(Citizen citizen)
        {
            throw new NotImplementedException();
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            throw new NotImplementedException();
        }
    }
}
