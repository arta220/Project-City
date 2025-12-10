using Domain.Common.Enums;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Services.Materials
{
    /// <summary>
    /// –еализаци€ сервиса управлени€ инвентарем материалов, предоставл€ющего методы дл€ работы с природными ресурсами
    /// </summary>
    public class MaterialInventoryService : IMaterialInventoryService
    {
        private readonly ConcurrentDictionary<NaturalResourceType, int> _store = new();

        /// <summary>
        /// »нициализаци€ нового экземпл€ра класса <see cref="MaterialInventoryService"/>
        /// </summary>
        /// <remarks>
        /// —оздание инвентар€ со всеми типами природных ресурсов, установка начального количества каждого типа в 0
        /// »сключение типа <see cref="NaturalResourceType.None"/> из инициализации
        /// </remarks>
        public MaterialInventoryService()
        {
            foreach (NaturalResourceType t in System.Enum.GetValues(typeof(NaturalResourceType)))
            {
                if (t == NaturalResourceType.None) continue;
                _store[t] = 0;
            }
        }

        /// <summary>
        /// ѕолучение всех материалов и их количество из инвентар€
        /// </summary>
        /// <returns>—ловарь только дл€ чтени€, содержащий типы природных ресурсов и их количество</returns>
        public IReadOnlyDictionary<NaturalResourceType, int> GetAll()
        {
            return new Dictionary<NaturalResourceType, int>(_store);
        }

        /// <summary>
        /// ѕолучение количества материала указанного типа
        /// </summary>
        /// <param name="type">“ип природного ресурса</param>
        /// <returns> оличество материала указанного типа</returns>
        public int GetQuantity(NaturalResourceType type)
        {
            return _store.TryGetValue(type, out var v) ? v : 0;
        }

        /// <summary>
        /// ƒобавление указанного количества материала в инвентарь
        /// </summary>
        /// <param name="type">“ип природного ресурса</param>
        /// <param name="amount"> оличество материала дл€ добавлени€</param>
        /// <remarks>
        /// ≈сли количество меньше или равно 0, метод не выполн€ет никаких действий
        /// </remarks>
        public void Add(NaturalResourceType type, int amount)
        {
            if (amount <= 0) return;
            _store.AddOrUpdate(type, amount, (_, prev) => prev + amount);
        }

        /// <summary>
        /// ”даление указанного количества материала из инвентар€
        /// </summary>
        /// <param name="type">“ип природного ресурса</param>
        /// <param name="amount"> оличество материала дл€ удалени€</param>
        /// <returns>
        /// true, если материал успешно удален (достаточное количество доступно);
        /// false, если материала недостаточно
        /// </returns>
        /// <remarks>
        /// ≈сли количество меньше или равно 0, метод возвращает true без изменений
        /// </remarks>
        public bool Remove(NaturalResourceType type, int amount)
        {
            if (amount <= 0) return true;
            bool success = false;
            _store.AddOrUpdate(type, 0, (_, prev) =>
            {
                if (prev >= amount)
                {
                    success = true;
                    return prev - amount;
                }
                return prev;
            });
            return success;
        }
    }
}