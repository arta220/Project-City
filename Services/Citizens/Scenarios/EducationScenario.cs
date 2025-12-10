using Domain.Citizens;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.EntityMovement.Service;
using Services.Time;
using System.Linq;

namespace Services.Citizens.Scenarios
{
    public class EducationScenario : ICitizenScenario
    {
        private readonly IEntityMovementService _movement;
        private readonly IBuildingRegistry _registry;
        private readonly MapModel _map;

        public EducationScenario(IEntityMovementService movement, IBuildingRegistry registry, MapModel map)
        {
            _movement = movement;
            _registry = registry;
            _map = map;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Сценарий применим только к детям и молодежи, которые ещё должны учиться
            return citizen.Age >= 7 && citizen.Age < 25;
        }

        public void BuildTasks(Citizen citizen)
        {
            if (citizen.StudyPlace == null)
            {
                // Добавляем задачу поиска учебного заведения
                citizen.Tasks.Enqueue(new FindEducationTask(_registry));

                // Добавляем задачу посещения учебного заведения
                citizen.Tasks.Enqueue(new AttendEducationTask(_movement, _registry, _map));
            }
            else
            {
                // Если учебное заведение уже есть, сразу идем туда
                citizen.Tasks.Enqueue(new AttendEducationTask(_movement, _registry, _map));
            }
        }
    }
}
