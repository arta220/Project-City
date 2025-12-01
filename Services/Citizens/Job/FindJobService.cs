using Services.Interfaces;
using Domain.Citizens.States;
using Domain.Common.Base;
using Services.BuildingRegistry;
using Domain.Buildings;

namespace Services.Citizens.Job
{
    public class FindJobService : IFindJobService
    {
        private readonly IBuildingRegistry _registry;

        public FindJobService(IBuildingRegistry registry) => _registry = registry;

        public IEnumerable<CommercialBuilding> FindJob(CitizenProfession profession)
        {
            return _registry.GetBuildings<CommercialBuilding>() // Пока только для коммерции
                            .Where(b => b.HasVacancy(profession));
        }
    }
}
