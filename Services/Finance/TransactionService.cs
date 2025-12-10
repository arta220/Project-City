using System.Collections.Generic;
using System.Linq;
using Domain.Finance;

namespace Services.Finance
{
    /// <summary>
    /// Реализация сервиса управления транзакциями, предоставляющего методы для регистрации и анализа финансовых операций
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        /// <summary>
        /// Регистрация новой финансовой транзакции
        /// </summary>
        /// <param name="transaction">Транзакция для регистрации</param>
        public void RegisterTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
        }

        /// <summary>
        /// Получение истории всех зарегистрированных транзакций
        /// </summary>
        /// <returns>Коллекция всех транзакций только для чтения</returns>
        public IEnumerable<Transaction> GetHistory()
        {
            return _transactions.AsReadOnly();
        }

        /// <summary>
        /// Рассчет общей суммы доходов за указанный период времени
        /// </summary>
        /// <param name="startTime">Начальное время периода</param>
        /// <param name="endTime">Конечное время периода</param>
        /// <returns>Общая сумма доходов за период</returns>
        public float GetTotalIncome(long startTime, long endTime)
        {
            return _transactions
                .Where(t => t.Timestamp >= startTime && t.Timestamp <= endTime && t.Type == TransactionType.Income)
                .Sum(t => t.Amount);
        }

        /// <summary>
        /// Рассчет общей суммы расходов за указанный период времени
        /// </summary>
        /// <param name="startTime">Начальное время периода </param>
        /// <param name="endTime">Конечное время периода </param>
        /// <returns>Общая сумма расходов за период</returns>
        public float GetTotalExpense(long startTime, long endTime)
        {
            return _transactions
                .Where(t => t.Timestamp >= startTime && t.Timestamp <= endTime && t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);
        }
    }
}