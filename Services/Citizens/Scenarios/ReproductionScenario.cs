using Domain.Citizens;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.Citizens.Tasks.Demography;
using Services.Time;

namespace Services.Citizens.Population
{
    public class ReproductionScenario : ICitizenScenario
    {
        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Пока просто проверяем возраст для потенциального воспроизводства
            return citizen.Age >= 18 && citizen.Age <= 40;
        }

        public void BuildTasks(Citizen citizen)
        {
            // Добавляем задачу рождения ребенка
            citizen.Tasks.Enqueue(new BirthTask(citizen));

            // Добавляем задачу проверки смертности
            citizen.Tasks.Enqueue(new DeathCheckTask(citizen));
        }
    }
}
