using Services.Time;
using Services.Common;
using System;
using Domain.Finance;
using System.Collections.Generic;

namespace Services.Finance
{
    /// <summary>
    /// Реализация сервиса управления финансами, отвечающего за операции с бюджетом, учет транзакций и сбор статистики
    /// </summary>
    public class FinanceService : IFinanceService, IUpdatable
    {
        private readonly ISimulationTimeService _timeService;
        private readonly Dictionary<string, float> _categoryTotals = new Dictionary<string, float>();
        private float _totalIncome = 0;
        private float _totalExpense = 0;
        private int _lastRecordedTick = -1;

        /// <summary>
        /// Получает текущий бюджет города
        /// </summary>
        public Budget Budget { get; }

        /// <summary>
        /// Получает статистику финансовых операций
        /// </summary>
        public FinanceStatistics Statistics { get; } = new FinanceStatistics();

        /// <summary>
        /// Событие, возникающее при изменении бюджета
        /// </summary>
        public event Action<float> BudgetChanged;

        /// <summary>
        /// Получает сервис управления транзакциями
        /// </summary>
        public ITransactionService TransactionService { get; }

        /// <summary>
        /// Получает сервис управления налогами
        /// </summary>
        public ITaxService TaxService { get; }

        /// <summary>
        /// Инициализация нового экземпляра класса <see cref="FinanceService"/>
        /// </summary>
        /// <param name="timeService">Сервис времени симуляции</param>
        public FinanceService(ISimulationTimeService timeService)
        {
            _timeService = timeService;
            Budget = new Budget { Balance = 8000000 };
            TransactionService = new TransactionService();
            TaxService = new TaxService();

            RecordStatistics(0);
        }

        /// <summary>
        /// Обновление состояния финансового сервиса с записью статистики на каждом новом тике
        /// </summary>
        /// <param name="time">Текущее время симуляции</param>
        public void Update(Domain.Common.Time.SimulationTime time)
        {
            if (time.TotalTicks != _lastRecordedTick)
            {
                RecordStatistics(time.TotalTicks);
                _lastRecordedTick = time.TotalTicks;
            }
        }

        /// <summary>
        /// Добавление дохода в бюджет
        /// </summary>
        /// <param name="amount">Сумма дохода</param>
        /// <param name="category">Категория дохода</param>
        /// <param name="description">Описание дохода</param>
        public void AddIncome(float amount, IncomeCategory category, string description)
        {
            Budget.Balance += amount;
            _totalIncome += amount;
            UpdateCategoryTotal(TransactionType.Income, category.ToString(), amount);

            BudgetChanged?.Invoke(amount);

            int currentTick = _timeService.CurrentTime.TotalTicks;
            TransactionService.RegisterTransaction(new Transaction
            {
                Amount = amount,
                Type = TransactionType.Income,
                Category = category.ToString(),
                Description = description,
                Timestamp = currentTick
            });

            RecordStatistics(currentTick);
        }

        /// <summary>
        /// Добавление расхода из бюджета
        /// </summary>
        /// <param name="amount">Сумма расхода</param>
        /// <param name="category">Категория расхода</param>
        /// <param name="description">Описание расхода</param>
        public void AddExpense(float amount, ExpenseCategory category, string description)
        {
            Budget.Balance -= amount;
            _totalExpense += amount;
            UpdateCategoryTotal(TransactionType.Expense, category.ToString(), amount);

            BudgetChanged?.Invoke(-amount);

            int currentTick = _timeService.CurrentTime.TotalTicks;
            TransactionService.RegisterTransaction(new Transaction
            {
                Amount = amount,
                Type = TransactionType.Expense,
                Category = category.ToString(),
                Description = description,
                Timestamp = currentTick
            });

            RecordStatistics(currentTick);
        }

        /// <summary>
        /// Обновление общей суммы по категории финансовой операции
        /// </summary>
        /// <param name="type">Тип транзакции (доход/расход)</param>
        /// <param name="category">Категория транзакции</param>
        /// <param name="amount">Сумма для добавления</param>
        private void UpdateCategoryTotal(TransactionType type, string category, float amount)
        {
            string key = $"{type}: {category}";
            if (!_categoryTotals.ContainsKey(key))
            {
                _categoryTotals[key] = 0;
            }
            _categoryTotals[key] += amount;
        }

        /// <summary>
        /// Запись текущей статистики финансов в историю
        /// </summary>
        /// <param name="tick">Номер тика симуляции</param>
        private void RecordStatistics(int tick)
        {
            Statistics.History.Add(new FinanceDataPoint(
                tick,
                Budget.Balance,
                _totalIncome,
                _totalExpense,
                _categoryTotals
            ));
        }
    }
}