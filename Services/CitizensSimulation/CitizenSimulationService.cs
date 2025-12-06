using Domain.Citizens;
using Domain.Common.Time;
using Services.Citizens.Movement;
using Services.Citizens.Population;
using Services.CitizensSimulation.CitizenSchedule;
using Services.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Services.CitizensSimulation
{
    /// <summary>
    /// Сервис симуляции граждан. Управляет коллекцией граждан и обновляет их состояние на каждом тике симуляции.
    /// </summary>
    public class CitizenSimulationService : IUpdatable
    {
        private int _lastProcessedYear = -1;
        private readonly CitizenController _controller;
        private readonly ICitizenScheduler _scheduler;
        private readonly IPopulationService _populationService;
        private readonly ICitizenMovementService _movementService;

        private bool _isPaused = true; // по умолчанию при старте приостановлен

        public ObservableCollection<Citizen> Citizens { get; } = new();

        public Action<Citizen> CitizenAdded;
        public Action<Citizen> CitizenRemoved;
        public Action<Citizen> CitizenUpdated;

        public CitizenSimulationService(
            CitizenController controller,
            IPopulationService populationService,
            ICitizenScheduler scheduler,
            ICitizenMovementService movementService)
        {
            _movementService = movementService;
            _scheduler = scheduler;
            _controller = controller;
            _populationService = populationService;
        }

        /// <summary>
        /// Основной метод обновления симуляции. Вызывается каждый тик.
        /// </summary>
        public void Update(SimulationTime time)
        {
            if (_isPaused) return;

            foreach (var citizen in Citizens)
            {
                _scheduler.UpdateSchedule(citizen);
                _controller.UpdateCitizen(citizen, time);
                CitizenUpdated?.Invoke(citizen);

                if (time.Year != _lastProcessedYear)
                {
                    _populationService.ProcessDemography(
                        Citizens.ToList(), time, CitizenAdded, CitizenRemoved);
                    _lastProcessedYear = time.Year;
                }
            }
        }

        public void AddCitizen(Citizen citizen)
        {
            if (citizen == null) return;
            Citizens.Add(citizen);
            CitizenAdded?.Invoke(citizen);
        }

        public void RemoveCitizen(Citizen citizen)
        {
            if (citizen == null) return;
            Citizens.Remove(citizen);
            CitizenRemoved?.Invoke(citizen);
        }

        /// <summary>
        /// Возобновляет симуляцию после полной загрузки UI или после паузы.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Ставит симуляцию на паузу.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }
    }
}
