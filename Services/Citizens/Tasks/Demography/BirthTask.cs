using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;

namespace Services.Citizens.Tasks.Demography
{
    public class BirthTask : ICitizenTask
    {
        private bool _isCompleted;
        public bool IsCompleted => _isCompleted;

        private readonly Citizen _parent;

        public BirthTask(Citizen parent) => _parent = parent;

        public void Execute(Citizen citizen, SimulationTime time)
        {
            // Заглушка — логика рождения потомка
            _isCompleted = true;
        }
    }
}
