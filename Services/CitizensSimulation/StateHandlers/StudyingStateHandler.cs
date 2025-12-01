using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Time;
using Services.Citizens.Education;

namespace Services.CitizensSimulation.StateHandlers
{
    /// <summary>
    /// Обработчик состояния "Studying" для жителя
    /// </summary>
    public class StudyingStateHandler : ICitizenStateHandler
    {
        private readonly IEducationService _educationService;

        public StudyingStateHandler(IEducationService educationService)
        {
            _educationService = educationService;
        }

        public bool CanHandle(CitizenState state) => state == CitizenState.Studying;

        public void Update(Citizen citizen, SimulationTime time)
        {
            if (citizen.StudyPlace == null)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            var levelBefore = citizen.EducationLevel;
            _educationService.UpdateEducation(citizen, time);

            if (citizen.EducationLevel > levelBefore)
            {
                citizen.State = CitizenState.Idle;
            }
        }
    }
}
