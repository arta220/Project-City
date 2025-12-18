using Domain.Buildings.EducationBuildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Time;

namespace Services.Citizens.Tasks
{
    public class StudyTask : ICitizenTask
    {
        private readonly EducationBuilding _building;
        private bool _started = false;
        private int _ticksRemaining;

        public bool IsCompleted { get; private set; }

        public StudyTask(EducationBuilding building)
        {
            _building = building;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted) return;

            if (!_started)
            {
                // start studying: register student if capacity
                if (!_building.HasCapacity)
                {
                    // no capacity -> finish immediately
                    IsCompleted = true;
                    return;
                }

                _building.AddStudent(citizen);
                citizen.StudyPlace = _building;
                citizen.State = CitizenState.Studying;

                // Set study duration based on education type
                _ticksRemaining = _building.Type switch
                {
                    Domain.Common.Enums.EducationType.School => 60,
                    Domain.Common.Enums.EducationType.College => 120,
                    Domain.Common.Enums.EducationType.University => 240,
                    _ => 60
                };

                _started = true;
                return;
            }

            // studying in progress
            _ticksRemaining--;

            if (_ticksRemaining <= 0)
            {
                // finish studying
                citizen.EducationLevel = _building.Type;
                citizen.State = CitizenState.Idle;
                IsCompleted = true;

                // remove student from building (they stay graduated but no longer count as current student)
                _building.RemoveStudent(citizen);
                citizen.StudyPlace = null;
            }
        }
    }
}
