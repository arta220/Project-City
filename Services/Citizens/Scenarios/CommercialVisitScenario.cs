using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Services.BuildingRegistry;
using Services.Citizens.Scenarios.Base;
using Services.Citizens.Tasks.Job;
using Services.CommercialVisits;
using Services.EntityMovement.Service;
using Services.Time;

namespace Services.Citizens.Scenarios
{
    public class CommercialVisitScenario : MoveAndPerformTaskScenario
    {
        private readonly ICommercialVisitService _visitService;
        private readonly Random _random = new Random();

        public CommercialVisitScenario(
            IEntityMovementService movement,
            IBuildingRegistry registry,
            ICommercialVisitService visitService)
            : base(movement, registry)
        {
            _visitService = visitService;
        }

        public override bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            if (citizen.Home == null)
                return false;

            var (placement, ok) = _registry.TryGetPlacement(citizen.Home);
            return ok && citizen.Position == placement.Value.Entrance;
        }

        protected override Building GetTargetBuilding(Citizen citizen)
        {
            var list = _registry.GetBuildings<CommercialBuilding>()
                                .Where(b => b.CanAcceptMoreVisitors)
                                .ToList();

            return list[_random.Next(list.Count)];
        }

        protected override ICitizenTask CreateMainTask(Citizen citizen, Building target)
        {
            return new CommercialServiceTask((CommercialBuilding)target, _visitService);
        }

        protected override Building? GetReturnTarget(Citizen citizen)
        {
            return citizen.Home;
        }
    }
}
