using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Time;

namespace Services.Citizens.Tasks.Job
{
    public class FinishWorkTask : ICitizenTask // Задача вызывается, когда работа "выполнена"
    {
        private readonly Citizen _citizen;

        public bool IsCompleted { get; private set; } = false;

        public FinishWorkTask(Citizen citizen)
        {
            _citizen = citizen;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            // Можно либо домой идти, либо просто дать жителю снова делать выбор чо он хочет
            _citizen.State = CitizenState.Idle;
            IsCompleted = true;
        }
    }
}
