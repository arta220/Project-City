using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;
using Services.BuildingRegistry;

namespace Services.Citizens.Tasks.Job
{

    public class FindJobTask : ICitizenTask // Задача поиска рабочего места для жителя
    {
        private readonly IBuildingRegistry _registry;
        private bool _isCompleted;

        public bool IsCompleted => _isCompleted;

        public FindJobTask(IBuildingRegistry registry)
        {
            _registry = registry;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (_isCompleted) return;

            if (citizen.WorkPlace != null)
            {
                _isCompleted = true;
                return;
            }

            var possibleJobs = _registry.GetBuildings<Building>()
                                        .Where(b => b.HasVacancy(citizen.Profession));

            var job = possibleJobs.FirstOrDefault();
            if (job != null)
                citizen.WorkPlace = job;

            _isCompleted = true;
        }
    }
}
