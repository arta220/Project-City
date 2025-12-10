namespace Services.Finance
{
    /// <summary>
    /// Определение сервиса управления налогами, предоставляющего методы для расчета налогов и управления налоговыми ставками
    /// </summary>
    public interface ITaxService
    {
        /// <summary>
        /// Рассчет суммы налога для указанной суммы дохода
        /// </summary>
        /// <param name="amount">Сумма, с которой рассчитывается налог</param>
        /// <param name="taxName">Название налога для расчета</param>
        /// <returns>Сумма налога к уплате</returns>
        float CalculateTax(float amount, string taxName);

        /// <summary>
        /// Получение текущей ставки указанного налога
        /// </summary>
        /// <param name="taxName">Название налога</param>
        /// <returns>Текущая ставка налога</returns>
        float GetTaxRate(string taxName);

        /// <summary>
        /// Установка новой ставки для указанного налога
        /// </summary>
        /// <param name="taxName">Название налога</param>
        /// <param name="rate">Новая ставка налога</param>
        void SetTaxRate(string taxName, float rate);
    }
}