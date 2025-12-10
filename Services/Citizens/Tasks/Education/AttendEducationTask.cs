using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.EntityMovement.Service;

namespace Services.Citizens.Tasks.Education
{
    public class AttendEducationTask : ICitizenTask // Задача посещения и обучения в учебном заведении
    {
        private readonly IEntityMovementService _movement;
        private readonly IBuildingRegistry _registry;
        private readonly MapModel _map;

        private bool _pathSet = false;
        private bool _isCompleted = false;

        public bool IsCompleted => _isCompleted;

        public AttendEducationTask(IEntityMovementService movement, IBuildingRegistry registry, MapModel map)
        {
            _movement = movement;
            _registry = registry;
            _map = map;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (_isCompleted || citizen.StudyPlace == null)
                return;

            if (!_pathSet)
            {
                var accessibleTiles = _registry.GetAccessibleNeighborTiles(citizen.StudyPlace, _map)
                                               .Where(pos => citizen.NavigationProfile.CanEnter(pos))
                                               .ToList();

                if (!accessibleTiles.Any())
                {
                    citizen.State = Domain.Citizens.States.CitizenState.Idle;
                    _isCompleted = true;
                    return;
                }

                _movement.SetTarget(citizen, accessibleTiles.First());
                _pathSet = true;
            }

            _movement.PlayMovement(citizen, time);

            if (citizen.Position == citizen.TargetPosition)
            {
                // Повышаем уровень образования по возрасту
                var newLevel = GetEducationLevelForAge(citizen.Age);
                if (citizen.EducationLevel < newLevel)
                    citizen.EducationLevel = newLevel;

                _isCompleted = true;
            }
        }

        private Domain.Common.Enums.EducationType GetEducationLevelForAge(int age) => age switch
        {
            >= 7 and < 15 => Domain.Common.Enums.EducationType.School,
            >= 15 and < 18 => Domain.Common.Enums.EducationType.College,
            >= 18 => Domain.Common.Enums.EducationType.University,
            _ => Domain.Common.Enums.EducationType.None
        };
    }
}
