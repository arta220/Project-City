using Domain.Citizens;
using Domain.Common.Time;
using Services.Citizens.Population;
using Services.CitizensSimulatiom;
using Services.CitizensSimulation.CitizenSchedule;
using Services.Common;
using Services.Interfaces;
using Services.Finance;
using Services.EntityMovement.Service;
using System.Collections.ObjectModel;

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
        private readonly IFinanceService _financeService;
        private int _lastProcessedMonth = -1;

        private bool _isPaused = true; // по умолчанию при старте приостановлен

        public ObservableCollection<Citizen> Citizens { get; } = new();

        public Action<Citizen> CitizenAdded;
        public Action<Citizen> CitizenRemoved;
        public Action<Citizen> CitizenUpdated;

        public CitizenSimulationService(
            CitizenController controller,
            IPopulationService populationService,
            IFinanceService financeService,
            ICitizenScheduler scheduler)
        {
            _scheduler = scheduler;
            _controller = controller;
            _populationService = populationService;
            _financeService = financeService;
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

            if (_lastProcessedMonth != time.Month)
            {
                _lastProcessedMonth = time.Month;
                ProcessMonthlyFinances();
            }
        }

        /// <summary>
        /// Обработка ежемесячных финансовых операций для всех граждан города
        /// </summary>
        /// <remarks>
        /// Метод выполняет следующие действия для каждого гражданина:
        /// 1. Начисляет заработную плату
        /// 2. Рассчитывает и удерживает подоходный налог
        /// 3. Добавляет собранный налог в бюджет города
        /// </remarks>
        private void ProcessMonthlyFinances()
        {
            var taxRate = _financeService.TaxService.GetTaxRate("Подоходный налог");
            foreach (var citizen in Citizens)
            {
                float salary = 1000; 
                citizen.Money += salary;

                // Налог
                float tax = salary * taxRate;
                citizen.Money -= tax;
                _financeService.AddIncome(tax, Domain.Finance.IncomeCategory.Tax, $"Налог на доход от {citizen.GetHashCode()}");
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
