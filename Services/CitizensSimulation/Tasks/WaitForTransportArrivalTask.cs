using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CitizensSimulation.Tasks
{
    /// <summary>
    /// Задача ожидания прибытия транспорта (для пассажиров)
    /// </summary>
    public class WaitForTransportArrivalTask : ICitizenTask
    {
        private readonly Transport _transport;
        private readonly Position _destination;

        public bool IsCompleted { get; private set; }

        public WaitForTransportArrivalTask(Transport transport, Position destination)
        {
            _transport = transport;
            _destination = destination;
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            // Синхронизируем позицию пассажира с транспортом
            citizen.Position = _transport.Position;

            // Проверяем, достиг ли транспорт цели
            if (_transport.HasReachedCurrentTarget() &&
                _transport.CurrentTarget == _destination)
            {
                IsCompleted = true;
                // Пассажир может выйти из транспорта
                citizen.CurrentTransport?.Passengers.Remove(citizen);
                citizen.CurrentTransport = null;
            }
        }
    }
}
