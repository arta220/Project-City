using Domain.Base;
using Domain.Map;

namespace Services.PlaceBuilding
{
    /// <summary>
    /// Репозиторий для отслеживания размещения объектов на карте.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется сервисами типа <see cref="BuildingRegistryService"/> и <see cref="MapObjectPlacementService"/>
    ///   для получения позиции зданий или других объектов на карте.
    /// - Хранит соответствие между <see cref="MapObject"/> и его <see cref="Placement"/>.
    /// - Позволяет расширять функционал, например, добавляя историю изменений или проверку пересечений.
    /// </remarks>
    public class PlacementRepository
    {
        private readonly Dictionary<MapObject, Placement> _placements = new();

        /// <summary>
        /// Регистрирует объект на указанной позиции.
        /// </summary>
        /// <param name="obj">Объект карты, который нужно разместить.</param>
        /// <param name="placement">Позиция и размер размещения.</param>
        public void Register(MapObject obj, Placement placement)
        {
            _placements[obj] = placement;
        }

        /// <summary>
        /// Получает размещение объекта на карте.
        /// </summary>
        /// <param name="obj">Объект карты.</param>
        /// <returns>Позиция и размер размещения.</returns>
        /// <exception cref="InvalidOperationException">Если объект не зарегистрирован.</exception>
        public Placement GetPlacement(MapObject obj)
        {
            if (!_placements.TryGetValue(obj, out var placement))
                throw new InvalidOperationException("Объект не размещён на карте.");
            return placement;
        }

        /// <summary>
        /// Удаляет объект из репозитория.
        /// </summary>
        /// <param name="obj">Объект карты.</param>
        public void Remove(MapObject obj)
        {
            _placements.Remove(obj);
        }

        /// <summary>
        /// Получает все зарегистрированные объекты на карте.
        /// </summary>
        /// <returns>Коллекция объектов карты.</returns>
        public IEnumerable<MapObject> GetAll() => _placements.Keys;
    }
}
