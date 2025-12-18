using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Citizens.Tasks;
using Services.Citizens.Education;
using Services.Time;
using Services.EntityMovement.Service;

namespace Services.Citizens.Scenaries
{
    public class HomeScenario : ICitizenScenario
    {
        private readonly IBuildingRegistry _registry;
        private readonly IEntityMovementService _movement;
        private readonly IFindEducationService _educationFinder;
        private readonly IEducationService _educationService;

        public HomeScenario(
            IBuildingRegistry registry,
            IEntityMovementService movement,
            IFindEducationService educationFinder,
            IEducationService educationService)
        {
            _registry = registry;
            _movement = movement;
            _educationFinder = educationFinder;
            _educationService = educationService;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            if (!(time.IsNightTime() || time.GetTimeOfDay() == TimeOfDay.Evening))
                return false;

            if (citizen.Home == null)
                return false;

            return !IsAtHome(citizen);
        }

        public void BuildTasks(Citizen citizen)
        {
            citizen.State = CitizenState.GoingHome;

            citizen.Tasks.Enqueue(
                new MoveToBuildingTask(citizen.Home!, _movement, _registry)
            );

            // Если возраст ребенка попадает в учебный период, планируем поход в школу/колледж/университет утром
            var required = GetRequiredEducationLevel(citizen.Age);
            if (required != Domain.Common.Enums.EducationType.None && citizen.StudyPlace == null)
            {
                var educBuildings = _educationFinder.FindEducation(citizen).ToList();
                if (educBuildings.Any())
                {
                    var building = educBuildings.First();
                    citizen.Tasks.Enqueue(new MoveToBuildingTask(building, _movement, _registry));
                    citizen.Tasks.Enqueue(new StudyTask(building));
                }
            }
        }

        private Domain.Common.Enums.EducationType GetRequiredEducationLevel(int age)
        {
            return age switch
            {
                >= 7 and < 15 => Domain.Common.Enums.EducationType.School,
                >= 15 and < 18 => Domain.Common.Enums.EducationType.College,
                >= 18 => Domain.Common.Enums.EducationType.University,
                _ => Domain.Common.Enums.EducationType.None
            };
        }

        private bool IsAtHome(Citizen citizen)
        {
            var (pl, ok) = _registry.TryGetPlacement(citizen.Home!);
            return ok && citizen.Position == pl.Value.Entrance;
        }
    }
}
