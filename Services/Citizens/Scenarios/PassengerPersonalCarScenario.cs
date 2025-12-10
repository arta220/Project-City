using Domain.Citizens;
using Services.Citizens.Scenaries;
using Services.EntityMovement.Service;
using Services.Time;
using Services.TransportSimulation;
using Services.CitizensSimulation.Tasks;

namespace Services.Citizens.Scenarios
{
    /// <summary>
    /// Сценарий для пассажира в личном транспорте
    /// </summary>
    public class PassengerPersonalCarScenario : ICitizenScenario
    {
        private readonly IEntityMovementService _movementService;
        private readonly RideSessionService _rideSessionService;

        public PassengerPersonalCarScenario(
            IEntityMovementService movementService,
            RideSessionService rideSessionService)
        {
            _movementService = movementService;
            _rideSessionService = rideSessionService;
        }

        public bool CanRun(Citizen citizen, ISimulationTimeService time)
        {
            // Сценарий может запуститься, если:
            // 1. Гражданин находится в личном транспорте как пассажир
            // 2. Транспорт имеет водителя
            // 3. Транспорт имеет целевую точку
            if (citizen.CurrentTransport == null)
                return false;

            if (citizen.CurrentTransport.CurrentDriver == null)
                return false;

            if (citizen.CurrentTransport.CurrentTarget == null)
                return false;

            // Проверяем, что гражданин не водитель
            return citizen.CurrentTransport.CurrentDriver != citizen;
        }

        public void BuildTasks(Citizen citizen)
        {
            if (citizen.CurrentTransport == null)
                return;

            var transport = citizen.CurrentTransport;
            var destination = transport.CurrentTarget;

            if (destination == null)
                return;

            // Пассажир просто ждет, пока транспорт достигнет цели
            citizen.Tasks.Enqueue(new WaitForTransportArrivalTask(transport, destination.Value));
        }
    }
}
