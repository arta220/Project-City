using Domain.Citizens;
using Domain.Common.Time;
using Services.Interfaces;

namespace Services.Citizens.Population
{
    public class PopulationService : IPopulationService
    {
        private readonly Random _random = new Random();
        public void ProcessDemography(List<Citizen> citizens, SimulationTime time,
                                      Action<Citizen> onCitizenBorn, Action<Citizen> onCitizenDied)
        {
            foreach (var citizen in citizens)
                citizen.Age++;

            // * репродукция *

            // Смертность
            foreach (var citizen in citizens.ToList())
            {
                var deathChance = CalculateDeathChance(citizen.Age);
                if (_random.Next(100) < deathChance)
                {
                    onCitizenDied?.Invoke(citizen);
                }
            }

        }

        private double CalculateDeathChance(int age) => 0; // Заглушка
    }
}
