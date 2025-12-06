using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Time;

public class CitizenController
{
    public void UpdateCitizen(Citizen citizen, SimulationTime time)
    {
        // Берём следующую таску
        if (citizen.CurrentTask is null)
        {
            if (citizen.Tasks.Count > 0)
                citizen.CurrentTask = citizen.Tasks.Dequeue();
        }

        // Если таска есть — выполняем
        if (citizen.CurrentTask is not null)
        {
            citizen.CurrentTask.Execute(citizen, time);

            if (citizen.CurrentTask.IsCompleted)
                citizen.CurrentTask = null;
        }

        // ВЫЗОВ РАБОТЫ ДЛЯ UTILITYWORKER
        if (citizen.Profession == CitizenProfession.UtilityWorker)
        {
            if (citizen.State == CitizenState.Working)
            {
                Debug.WriteLine($"ВЫЗОВ РАБОТЫ: UtilityWorker {citizen.Id}");
                _jobController.UpdateJob(citizen, time);
            }
            else if (citizen.State == CitizenState.WorkingOnSite)
            {
                Debug.WriteLine($"ВЫЗОВ РАБОТЫ: UtilityWorker {citizen.Id} НА ВЫЕЗДЕ!");
                _jobController.UpdateJob(citizen, time);
            }
        }
    }
}