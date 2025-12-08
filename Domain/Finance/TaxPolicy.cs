using System.Collections.Generic;

namespace Domain.Finance
{
    /// <summary>
    /// Представляет налоговую политику, содержащую набор налогов
    /// Определяет все налоги, применяемые в финансовой системе
    /// </summary>
    public class TaxPolicy
    {
        /// <summary>
        /// Получает или задает список налогов, входящих в налоговую политику
        /// </summary>
        /// <value>
        /// Коллекция объектов <see cref="Tax"/>, представляющих различные виды налогов
        /// По умолчанию инициализируется пустым списком
        /// </value>
        public List<Tax> Taxes { get; set; } = new List<Tax>();
    }
}
