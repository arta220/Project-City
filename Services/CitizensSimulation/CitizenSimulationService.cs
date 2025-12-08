using Domain.Citizens;
using Domain.Common.Time;
using Services.Citizens.Population;
using Services.Common;
using Services.Interfaces;
using Services.Finance;
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
        private readonly IPopulationService _populationService;
        private readonly IFinanceService _financeService;
        private int _lastProcessedMonth = -1;
        public ObservableCollection<Citizen> Citizens { get; } = new();

        public Action<Citizen> CitizenAdded;
        public Action<Citizen> CitizenRemoved;
        public Action<Citizen> CitizenUpdated;

        public CitizenSimulationService(
            CitizenController controller,
            IPopulationService populationService,
            IFinanceService financeService)
        {
            _controller = controller;
            _populationService = populationService;
            _financeService = financeService;
        }
        public void Update(SimulationTime time)
        {
            foreach (var citizen in Citizens)
            {
                _controller.UpdateCitizen(citizen, time);
                CitizenUpdated?.Invoke(citizen);
                _populationService.ProcessDemography(Citizens.ToList(), time, CitizenAdded, CitizenRemoved);

                if (time.Year != _lastProcessedYear)
                {
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
    }
}
