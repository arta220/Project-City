using Domain.Citizens;
using Domain.Common.Time;
using Services.Time;

namespace Services.Citizens.Scenaries
{
    public interface ICitizenScenario
    {
        bool CanRun(Citizen citizen, ISimulationTimeService time);
        void BuildTasks(Citizen citizen);
    }
}
