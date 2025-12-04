using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Time;

namespace Services.CitizensSimulation.CitizenSchedule
{
    public class CitizenScheduler : ICitizenScheduler
    {
        private readonly ISimulationTimeService _time;
        private readonly IBuildingRegistry _registry;

        public CitizenScheduler(ISimulationTimeService time, IBuildingRegistry registry)
        {
            _time = time;
            _registry = registry;
        }

        public void UpdateSchedule(Citizen citizen)
        {
            // Если уже есть задачи — не пересчитываем расписание
            if (citizen.CurrentTask != null || citizen.Tasks.Count > 0)
                return;

            var tod = _time.GetTimeOfDay();
            bool weekend = _time.IsWeekend();

            // Вечер — домой
            if ((_time.IsNightTime() || tod == TimeOfDay.Evening)
                && !IsAt(citizen, citizen.Home))
            {
                citizen.State = CitizenState.GoingHome;

                citizen.Tasks.Enqueue(new CitizenTask(
                    CitizenTaskType.MoveToPosition,
                    GetEntrance(citizen.Home)));

                return;
            }

            // Утро — работа
            if (tod == TimeOfDay.Morning && !weekend && citizen.WorkPlace != null)
            {
                if (!IsAt(citizen, citizen.WorkPlace))
                {
                    citizen.State = CitizenState.GoingWork;

                    citizen.Tasks.Enqueue(new CitizenTask(
                        CitizenTaskType.MoveToPosition,
                        GetEntrance(citizen.WorkPlace)));

                    citizen.Tasks.Enqueue(new CitizenTask(
                        CitizenTaskType.Work,
                        GetEntrance(citizen.WorkPlace)));

                    return;
                }
                else
                {
                    citizen.State = CitizenState.Working;
                    return;
                }
            }

            citizen.State = CitizenState.Idle;
        }

        private bool IsAt(Citizen citizen, MapObject building)
        {
            var (placement, ok) = _registry.TryGetPlacement(building);
            return ok && citizen.Position == placement.Value.Entrance;
        }

        private Position GetEntrance(MapObject building)
        {
            var (placement, _) = _registry.TryGetPlacement(building);
            return placement.Value.Entrance;
        }
    }
}
