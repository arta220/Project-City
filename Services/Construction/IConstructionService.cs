using Domain.Buildings.Construction;
using Domain.Common.Time;
using Services.Common;

namespace Services.Construction
{
    /// <summary>
    /// Интерфейс сервиса управления строительством
    /// </summary>
    public interface IConstructionService : IUpdatable
    {
        /// <summary>
        /// Событие завершения строительства
        /// </summary>
        event Action<ConstructionSite> ConstructionCompleted;

        /// <summary>
        /// Начинает строительство здания на указанной позиции
        /// </summary>
        bool StartConstruction(ConstructionSite constructionSite);

        /// <summary>
        /// Отменяет строительство
        /// </summary>
        bool CancelConstruction(ConstructionSite constructionSite);

        /// <summary>
        /// Получает все активные строительные площадки
        /// </summary>
        IEnumerable<ConstructionSite> GetActiveConstructionSites();
    }
}

