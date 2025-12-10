namespace Domain.Finance
{
    public class Tax
    {
        /// <summary>
        /// Получает или задает название налога
        /// Например: "Налог на недвижимость", "Подоходный налог", "НДС"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Получает или задает ставку налога в процентах или абсолютном значении
        /// Например: 0.13 (13%) для подоходного налога
        /// </summary>
        public float Rate { get; set; }

        /// <summary>
        /// Получает или задает категорию дохода, к которой относится налог
        /// Определяет тип дохода, с которого взимается данный налог
        /// </summary>
        public IncomeCategory Category
        {
            get; set;
        }
    }
}
