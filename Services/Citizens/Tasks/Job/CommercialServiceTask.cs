using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Services.CommercialVisits;

namespace Services.Citizens.Tasks.Job
{
    public class CommercialServiceTask : ICitizenTask
    {
        private readonly IServiceBuilding _building;
        private readonly ICommercialVisitService _visitService;
        private bool _queued;
        private bool _inside;
        private int _servedTicks;

        public bool IsCompleted { get; private set; }

        public CommercialServiceTask(IServiceBuilding building, ICommercialVisitService visitService)
        {
            _building = building;
            _visitService = visitService;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            if (!_inside)
            {
                _building.ProcessQueue();

                if (_building.TryEnter())
                {
                    if (_queued)
                        _building.LeaveQueue();

                    _inside = true;
                    _servedTicks = 0;
                    citizen.State = CitizenState.AtCommercial;
                }
                else
                {
                    if (!_queued)
                    {
                        _queued = _building.TryJoinQueue();
                    }

                    return;
                }
            }

            _servedTicks++;

            if (_servedTicks >= _building.ServiceTimeInTicks)
            {
                _building.Leave();
                _visitService.RecordVisit(_building.CommercialType, time.TotalTicks);
                citizen.State = CitizenState.GoingHomeFromCommercial;
                IsCompleted = true;
            }
        }
    }
}

