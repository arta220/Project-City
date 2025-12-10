using Domain.Citizens;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Services.TransportSimulation.RideSessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CitizensSimulation.Tasks
{
    /// <summary>
    /// Задача управления транспортом до конечной точки.
    /// Использует сессию поездки водителя для управления движением транспорта.
    /// </summary>
    public class DriveToDestinationTask : ICitizenTask
    {
        private readonly IDriverRideSession _rideSession;

        public bool IsCompleted => _rideSession.IsCompleted;

        public DriveToDestinationTask(IDriverRideSession rideSession)
        {
            _rideSession = rideSession ?? throw new ArgumentNullException(nameof(rideSession));
        }

        public void Execute(Citizen citizen, SimulationTime time)
        {
            if (IsCompleted)
                return;

            // Обновляем сессию поездки (транспорт движется)
            _rideSession.Update(time);

            // Синхронизируем позицию водителя с позицией транспорта
            if (citizen.CurrentTransport != null)
            {
                citizen.Position = citizen.CurrentTransport.Position;
            }
        }
    }
}
