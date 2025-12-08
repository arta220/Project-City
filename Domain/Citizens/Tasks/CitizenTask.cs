using Domain.Common.Time;

namespace Domain.Citizens.Tasks
{
    public interface ICitizenTask
    {
        bool IsCompleted { get; }
        void Execute(Citizen citizen, SimulationTime time);
    }

}