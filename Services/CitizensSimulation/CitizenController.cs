using Domain.Citizens;
using Domain.Common.Time;

public class CitizenController
{
    public void UpdateCitizen(Citizen citizen, SimulationTime time)
    {
        // Берём следующую таску
        if (citizen.CurrentTask == null)
        {
            if (citizen.Tasks.Count > 0)
                citizen.CurrentTask = citizen.Tasks.Dequeue();
        }

        // Если таска есть — выполняем
        if (citizen.CurrentTask != null)
        {
            citizen.CurrentTask.Execute(citizen, time);

            if (citizen.CurrentTask.IsCompleted)
                citizen.CurrentTask = null;
        }
    }
}
