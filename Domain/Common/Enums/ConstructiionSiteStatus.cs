using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Enums
{
    /// <summary>
    /// Статусы строительной площадки
    /// </summary>
    public enum ConstructionSiteStatus
    {
        /// <summary>
        /// Подготовка - ожидание материалов и рабочих
        /// </summary>
        Preparing,
        
        /// <summary>
        /// Строительство в процессе
        /// </summary>
        Building,
        
        /// <summary>
        /// Строительство завершено
        /// </summary>
        Completed,
        
        /// <summary>
        /// Строительство отменено
        /// </summary>
        Cancelled
    }
}
