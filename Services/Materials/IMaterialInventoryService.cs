using Domain.Common.Enums;
using System.Collections.Generic;

namespace Services.Materials
{
    /// <summary>
    /// ќпределение сервиса управлени€ инвентарем материалов, предоставл€ющего методы дл€ работы с природными ресурсами
    /// </summary>
    public interface IMaterialInventoryService
    {
        /// <summary>
        /// ѕолучает все материалы и их количество из инвентар€
        /// </summary>
        /// <returns>—ловарь только дл€ чтени€, содержащий типы природных ресурсов и их количество</returns>
        IReadOnlyDictionary<NaturalResourceType, int> GetAll();

        /// <summary>
        /// ѕолучает количество материала указанного типа
        /// </summary>
        /// <param name="type">“ип природного ресурса</param>
        /// <returns> оличество материала указанного типа</returns>
        int GetQuantity(NaturalResourceType type);

        /// <summary>
        /// ƒобавление указанного количества материала в инвентарь
        /// </summary>
        /// <param name="type">“ип природного ресурса</param>
        /// <param name="amount"> оличество материала дл€ добавлени€</param>
        void Add(NaturalResourceType type, int amount);

        /// <summary>
        /// ”даление указанного количества материала из инвентар€
        /// </summary>
        /// <param name="type">“ип природного ресурса</param>
        /// <param name="amount"> оличество материала дл€ удалени€</param>
        /// <returns>true, если материал успешно удален; false, если материала недостаточно</returns>
        bool Remove(NaturalResourceType type, int amount);
    }
}