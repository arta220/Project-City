using System.Linq;
using Domain.Finance;

namespace Services.Finance
{
    /// <summary>
    /// Реализация сервиса управления налогами, предоставляющего методы для расчета налогов и управления налоговыми ставками
    /// </summary>
    public class TaxService : ITaxService
    {
        private readonly TaxPolicy _taxPolicy;

        /// <summary>
        /// Инициализация нового экземпляра класса <see cref="TaxService"/> с предустановленными налогами
        /// </summary>
        /// <remarks>
        /// По умолчанию создаются три налога:
        /// 1. Подоходный налог - 13%
        /// 2. Налог на имущество - 1%
        /// 3. НДС - 20%
        /// </remarks>
        public TaxService()
        {
            _taxPolicy = new TaxPolicy();
            _taxPolicy.Taxes.Add(new Tax { Name = "Подоходный налог", Rate = 0.13f, Category = IncomeCategory.Tax });
            _taxPolicy.Taxes.Add(new Tax { Name = "Налог на имущество ", Rate = 0.01f, Category = IncomeCategory.Tax });
            _taxPolicy.Taxes.Add(new Tax { Name = "НДС", Rate = 0.20f, Category = IncomeCategory.Tax });
        }

        /// <summary>
        /// Рассчет суммы налога для указанной суммы дохода
        /// </summary>
        /// <param name="amount">Сумма, с которой рассчитывается налог</param>
        /// <param name="taxName">Название налога для расчета</param>
        /// <returns>
        /// Сумма налога к уплате. Возвращает 0, если налог с указанным названием не найден.
        /// </returns>
        public float CalculateTax(float amount, string taxName)
        {
            var tax = _taxPolicy.Taxes.FirstOrDefault(t => t.Name == taxName);
            if (tax == null) return 0;
            return amount * tax.Rate;
        }

        /// <summary>
        /// Получение текущей ставки указанного налога
        /// </summary>
        /// <param name="taxName">Название налога</param>
        /// <returns>
        /// Текущая ставка налога. Возвращает 0, если налог с указанным названием не найден.
        /// </returns>
        public float GetTaxRate(string taxName)
        {
            var tax = _taxPolicy.Taxes.FirstOrDefault(t => t.Name == taxName);
            return tax?.Rate ?? 0;
        }

        /// <summary>
        /// Устанавка новой ставки для указанного налога
        /// </summary>
        /// <param name="taxName">Название налога</param>
        /// <param name="rate">Новая ставка налога</param>
        public void SetTaxRate(string taxName, float rate)
        {
            var tax = _taxPolicy.Taxes.FirstOrDefault(t => t.Name == taxName);
            if (tax != null)
            {
                tax.Rate = rate;
            }
        }
    }
}