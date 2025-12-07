using Domain.Buildings.Residential;
using Domain.Common.Base;
using Domain.Map;
using System.Collections.Generic;
using System.Linq;

namespace Services.PlaceBuilding
{
    /// <summary>
    /// Репозиторий для отслеживания размещения объектов на карте.
    /// </summary>
    public class PlacementRepository
    {
        private readonly Dictionary<MapObject, Placement> _placements = new();

        /// <summary>
        /// Регистрирует объект на указанной позиции.
        /// </summary>
        public void Register(MapObject obj, Placement placement)
        {
            _placements[obj] = placement;
        }

        /// <summary>
        /// Получает размещение объекта на карте.
        /// </summary>
        public (Placement? placement, bool found) TryGetPlacement(MapObject obj)
        {
            if (obj == null) return (null, false);
            if (!_placements.TryGetValue(obj, out var placement)) return (null, false);

            return (placement, true);
        }

        /// <summary>
        /// Удаляет объект из репозитория.
        /// </summary>
        public void Remove(MapObject obj)
        {
            _placements.Remove(obj);
        }

        /// <summary>
        /// Получает все зарегистрированные объекты на карте.
        /// </summary>
        public IEnumerable<MapObject> GetAll() => _placements.Keys;

        /// <summary>
        /// Возвращает все жилые здания (ResidentialBuilding) на карте.
        /// </summary>
        public IEnumerable<ResidentialBuilding> GetAllResidentialBuildings()
        {
            return _placements.Keys.OfType<ResidentialBuilding>();
        }

        /// <summary>
        /// Проверяет, существует ли объект в репозитории.
        /// </summary>
        public bool Contains(MapObject obj) => _placements.ContainsKey(obj);
    }
}
