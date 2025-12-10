using Domain.Finance;
using System;

namespace Services.Finance
{
    /// <summary>
    /// Определение сервиса управления финансами, предоставляющего метода для работы с бюджетом и финансовыми операциями
    /// </summary>
    public interface IFinanceService
    {
        /// <summary>
        /// Получает текущий бюджет города
        /// </summary>
        Budget Budget { get; }

        /// <summary>
        /// Получает статистику финансовых операций
        /// </summary>
        FinanceStatistics Statistics { get; }

        /// <summary>
        /// Событие, возникающее при изменении бюджета
        /// </summary>
        event Action<float> BudgetChanged;

        /// <summary>
        /// Добавление дохода в бюджет
        /// </summary>
        /// <param name="amount">Сумма дохода</param>
        /// <param name="category">Категория дохода</param>
        /// <param name="description">Описание дохода</param>
        void AddIncome(float amount, IncomeCategory category, string description);

        /// <summary>
        /// Добавление расхода из бюджета
        /// </summary>
        /// <param name="amount">Сумма расхода</param>
        /// <param name="category">Категория расхода</param>
        /// <param name="description">Описание расхода</param>
        void AddExpense(float amount, ExpenseCategory category, string description);

        /// <summary>
        /// Получает сервис управления транзакциями
        /// </summary>
        ITransactionService TransactionService { get; }

        /// <summary>
        /// Получает сервис управления налогами
        /// </summary>
        ITaxService TaxService { get; }
    }
}