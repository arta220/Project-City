using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Map;

namespace Services.CitizensSimulation
{
    public class CitizenController
    {
        private readonly MovementService _movement;
        private readonly JobService _jobService;
        private readonly EducationService _educationService;
        private readonly PopulationService _populationService;

        public void UpdateCitizen(Citizen citizen)
        {
            switch (citizen.State)
            {
                case CitizenState.Idle:
                    DecideNextAction(citizen);
                    break;

                case CitizenState.GoingToWork:
                    _movement.Move(citizen, new Position(0, 0)); // Пока нет реализации работы жителя, нет зданий
                    break;

                case CitizenState.Working:
                    _jobService.UpdateWork(citizen);
                    break;

                case CitizenState.GoingToSchool:
                    _movement.Move(citizen, new Position(0, 0)); // Пока нет реализации учёбы жителя, нет зданий
                    break;

                case CitizenState.Studying:
                    _educationService.UpdateEducation(citizen);
                    break;

                case CitizenState.GoingHome:
                    _movement.Move(citizen, new Position(0, 0)); // Пока нет реализации дома жителя, нет зданий
                    break;
            }

            _populationService.AgeCitizen(citizen);
        }

        private void DecideNextAction(Citizen citizen)
        {

        }
    }

}
