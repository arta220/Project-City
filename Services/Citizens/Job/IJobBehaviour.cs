using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;

namespace Services.Citizens.Job
{
    public interface IJobBehaviour
    {
        CitizenProfession CitizenProfession { get; }
        void Update(Citizen citizen, SimulationTime time);
    }

}
