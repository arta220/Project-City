using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;

namespace Services.Citizens.Tasks
{
    public class ApplyForJobTask : ICitizenTask
    {
        private readonly Building _targetBuilding;
        public bool IsCompleted { get; private set; }

        public ApplyForJobTask(Building targetBuilding)
        {
            _targetBuilding = targetBuilding;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            // Пытаемся устроиться на работу
            var hired = _targetBuilding.Hire(citizen);

            if (hired)
            {
                citizen.State = CitizenState.Working;
            }
            else
            {
                citizen.State = CitizenState.Idle;
                // Вакансия уже занята или что-то пошло не так
            }

            IsCompleted = true;
        }
    }
}