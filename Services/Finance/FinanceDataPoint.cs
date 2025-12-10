using System.Collections.Generic;

namespace Services.Finance
{
    /// <summary>
    /// Представление точки данных финансовой статистики на определенный момент времени
    /// Содержит агрегированную информацию о финансовом состоянии на конкретном тике симуляции
    /// </summary>
    public class FinanceDataPoint
    {
        /// <summary>
        /// Получает или задает номер тика симуляции, к которому относится точка данных
        /// </summary>
        public int Tick { get; set; }

        /// <summary>
        /// Получает или задает общий баланс бюджета на момент тика
        /// </summary>
        public float Balance { get; set; }

        /// <summary>
        /// Получает или задает общую сумму доходов на момент тика
        /// </summary>
        public float TotalIncome { get; set; }

        /// <summary>
        /// Получает или задает общую сумму расходов на момент тика
        /// </summary>
        public float TotalExpense { get; set; }

        /// <summary>
        /// Получает или задает словарь с суммами по категориям финансовых операций
        /// </summary>
        /// <value>
        /// Словарь, где ключ - название категории, значение - сумма по этой категории
        /// </value>
        public Dictionary<string, float> CategoryTotals { get; set; } = new Dictionary<string, float>();

        /// <summary>
        /// Инициализация нового экземпляра класса <see cref="FinanceDataPoint"/> с указанными значениями
        /// </summary>
        /// <param name="tick">Номер тика симуляции</param>
        /// <param name="balance">Общий баланс бюджета</param>
        /// <param name="totalIncome">Общая сумма доходов</param>
        /// <param name="totalExpense">Общая сумма расходов</param>
        /// <param name="categoryTotals">Словарь с суммами по категориям</param>
        public FinanceDataPoint(int tick, float balance, float totalIncome, float totalExpense, Dictionary<string, float> categoryTotals)
        {
            Tick = tick;
            Balance = balance;
            TotalIncome = totalIncome;
            TotalExpense = totalExpense;
            CategoryTotals = new Dictionary<string, float>(categoryTotals);
        }
    }
}