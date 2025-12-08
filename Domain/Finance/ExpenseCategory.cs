namespace Domain.Finance
{

    public enum ExpenseCategory
    {
        /// <summary>
        /// Расходы на инфраструктуру: дороги, мосты, коммуникации
        /// </summary>
        Infrastructure,

        /// <summary>
        /// Расходы на городские службы и услуги
        /// </summary>
        Services,

        /// <summary>
        /// Расходы на заработную плату сотрудников
        /// </summary>
        Salaries,

        /// <summary>
        /// Расходы на строительство зданий и сооружений
        /// </summary>
        Construction,

        /// <summary>
        /// Расходы на техническое обслуживание и ремонт
        /// </summary>
        Maintenance,

        /// <summary>
        /// Прочие расходы, не попадающие в основные категории
        /// </summary>
        Other
    }
}