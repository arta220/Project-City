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
                // Если у жителя есть машина, он находится на работе и машина тоже там — едем домой на машине
                if (citizen.HasCar && citizen.WorkPlace != null && IsAt(citizen, citizen.WorkPlace))
                {
                    citizen.State = CitizenState.GoingHome;

                    // 1. Дойти пешком до машины (считаем, что она припаркована у работы)
                    citizen.Tasks.Enqueue(new CitizenTask(
                        CitizenTaskType.WalkToCar,
                        citizen.PersonalCar.Position));

                    // 2. Сесть в машину, а целевая точка для машины — вход в дом
                    citizen.Tasks.Enqueue(new CitizenTask(
                        CitizenTaskType.EnterCar,
                        GetEntrance(citizen.Home)));

                    return;
                }

                // Иначе — обычное поведение: пешком до дома
                citizen.State = CitizenState.GoingHome;

                citizen.Tasks.Enqueue(new CitizenTask(
                    CitizenTaskType.MoveToPosition,
                    GetEntrance(citizen.Home)));

                return;
            }

            // Утро — работа
            if (tod == TimeOfDay.Morning && !weekend && citizen.WorkPlace != null)
            {
                // Если у жителя есть машина и он сейчас дома — едем на работу на машине
                if (citizen.HasCar && citizen.Home != null && IsAt(citizen, citizen.Home))
                {
                    citizen.State = CitizenState.GoingWork;

                    // 1. Дойти пешком до машины (считаем, что она стоит рядом с домом)
                    citizen.Tasks.Enqueue(new CitizenTask(
                        CitizenTaskType.WalkToCar,
                        citizen.PersonalCar.Position));

                    // 2. Сесть в машину, а целевая точка для машины — вход в работу
                    citizen.Tasks.Enqueue(new CitizenTask(
                        CitizenTaskType.EnterCar,
                        GetEntrance(citizen.WorkPlace)));

                    return;
                }

                // Иначе — обычное поведение: пешком до работы
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
