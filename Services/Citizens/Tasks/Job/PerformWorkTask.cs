using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;

namespace Services.Citizens.Tasks.Job
{
    public class PerformWorkTask : ICitizenTask // Задача "выполнить" работу
    {
        private readonly Citizen _citizen;
        private int _ticksRemaining;

        public bool IsCompleted => _ticksRemaining <= 0;

        public PerformWorkTask(Citizen citizen)
        {
            _citizen = citizen;
            _ticksRemaining = 10;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (_ticksRemaining > 0)
                _ticksRemaining--;
        }
    }
}
