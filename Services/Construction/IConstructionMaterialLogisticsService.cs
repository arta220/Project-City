using Domain.Buildings;
using Domain.Buildings.Construction;
using Domain.Common.Time;
using Services.Common;

namespace Services.Construction
{
    /// <summary>
    /// Интерфейс сервиса логистики строительных материалов
    /// </summary>
    public interface IConstructionMaterialLogisticsService : IUpdatable
    {
        /// <summary>
        /// Запрашивает доставку материалов на строительную площадку
        /// </summary>
        void RequestMaterialsDelivery(ConstructionSite constructionSite);

        /// <summary>
        /// Находит ближайший источник материалов
        /// </summary>
        IndustrialBuilding FindMaterialSource(Enum materialType, Domain.Map.Position targetPosition);
    }
}

