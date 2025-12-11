using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Time;

namespace Services.Citizens.Tasks.Demography
{
    public class DeathCheckTask : ICitizenTask
    {
        private bool _isCompleted;
        public bool IsCompleted => _isCompleted;

        private readonly Citizen _citizen;
        private readonly Random _random = new Random();

        public DeathCheckTask(Citizen citizen) => _citizen = citizen;

        public void Execute(Citizen citizen, SimulationTime time)
        {
            // Заглушка — проверка смерти
            double deathChance = CalculateDeathChance(_citizen.Age);
            if (_random.NextDouble() < deathChance)
            {
                citizen.State = CitizenState.Dead;
            }

            _isCompleted = true;
        }

        private double CalculateDeathChance(int age) => 0; // Заглушка
    }
}
