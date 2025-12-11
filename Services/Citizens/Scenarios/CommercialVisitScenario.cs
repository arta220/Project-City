using System;
using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Services.BuildingRegistry;
using Services.Citizens.Scenaries;
using Services.Citizens.Tasks;
using Services.CommercialVisits;
using Services.EntityMovement.Service;
using Services.Time;

namespace Services.Citizens.Scenarios
{
    public class CommercialVisitScenario : ICitizenScenario
    {
        private readonly IBuildingRegistry _registry;
        private readonly IEntityMovementService _movement;
        private readonly ICommercialVisitService _visitService;
        private readonly Random _random = new();

        public CommercialVisitScenario(
            IBuildingRegistry registry,
            IEntityMovementService movement,
            ICommercialVisitService visitService)
        {
            _registry = registry;
            _movement = movement;
            _visitService = visitService;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            if (citizen.Home == null)
                return false;

            return IsAtHome(citizen);
        }

        public void BuildTasks(Citizen citizen)
        {
            var commercialBuildings = _registry.GetBuildings<CommercialBuilding>()
                                               .Where(b => b.CanAcceptMoreVisitors)
                                               .ToList();

            if (commercialBuildings.Count == 0)
            {
                citizen.State = CitizenState.Idle;
                return;
            }

            var target = commercialBuildings[_random.Next(commercialBuildings.Count)];

            citizen.State = CitizenState.GoingToCommercial;

            citizen.Tasks.Enqueue(new MoveToBuildingTask(target, _movement, _registry));
            citizen.Tasks.Enqueue(new CommercialServiceTask(target, _visitService));
            citizen.Tasks.Enqueue(new MoveToBuildingTask(citizen.Home!, _movement, _registry));
        }

        private bool IsAtHome(Citizen citizen)
        {
            var (placement, ok) = _registry.TryGetPlacement(citizen.Home!);
            return ok && citizen.Position == placement.Value.Entrance;
        }
    }
}

