using System.Collections.Generic;
using Domain.Finance;

namespace Services.Finance
{
    /// <summary>
    /// Определение сервиса управления транзакциями, предоставляющего методы для регистрации и анализа финансовых операций
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Регистрирация новой финансовой транзакции
        /// </summary>
        /// <param name="transaction">Транзакция для регистрации</param>
        void RegisterTransaction(Transaction transaction);

        /// <summary>
        /// Получение истории всех зарегистрированных транзакций
        /// </summary>
        /// <returns>Коллекция всех транзакций</returns>
        IEnumerable<Transaction> GetHistory();

        /// <summary>
        /// Рассчет общей суммы доходов за указанный период времени
        /// </summary>
        /// <param name="startTime">Начальное время периода</param>
        /// <param name="endTime">Конечное время периода</param>
        /// <returns>Общая сумма доходов за период</returns>
        float GetTotalIncome(long startTime, long endTime);

        /// <summary>
        /// Рассчет общей суммы расходов за указанный период времени
        /// </summary>
        /// <param name="startTime">Начальное время периода</param>
        /// <param name="endTime">Конечное время периода</param>
        /// <returns>Общая сумма расходов за период</returns>
        float GetTotalExpense(long startTime, long endTime);
    }
}