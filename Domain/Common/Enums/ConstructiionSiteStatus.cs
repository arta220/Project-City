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
        Preparing,       // Подготовка (ожидание материалов)
        Building,       // Строительство в процессе
        Completed,      // Строительство завершено
        Cancelled       // Строительство отменено
    }
}
