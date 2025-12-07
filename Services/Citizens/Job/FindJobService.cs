// Services/Citizens/Job/FindJobService.cs
using Domain.Buildings;
using Domain.Citizens.States;
using Domain.Common.Base;
using Services.BuildingRegistry;
using Services.Interfaces;
using System.Linq;

namespace Services.Citizens.Job
{
    public class FindJobService : IFindJobService
    {
        private readonly IBuildingRegistry _registry;

        public FindJobService(IBuildingRegistry registry) => _registry = registry;

        public IEnumerable<Building> FindJob(CitizenProfession profession)
        {
            // Получаем все здания с нужной профессией
            return _registry.GetBuildings<Building>()
                       .Where(b => b.HasVacancy(profession));
        }
    }
}