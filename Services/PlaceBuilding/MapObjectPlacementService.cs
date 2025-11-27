using Domain.Base;
using Services.Interfaces;
using Domain.Map;

namespace Services.PlaceBuilding
{
    /// <summary>
    /// Сервис размещения объектов на карте.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется для проверки возможности размещения зданий, дорог или других объектов на карте.
    /// - Интегрируется с <see cref="ConstructionValidator"/> для проверки правил строительства.
    /// - Вызывается из <see cref="Simulation"/> и UI (MapVM) при попытке поставить объект.
    /// 
    /// Возможные расширения:
    /// - Реализовать метод <see cref="RemoveBuilding"/> для удаления объектов.
    /// - Поддержка различных типов объектов с уникальными правилами размещения.
    /// - Логирование попыток размещения объектов для аналитики.
    /// </remarks>
    public class MapObjectPlacementService : IMapObjectPlacementService
    {
        private readonly ConstructionValidator _validator;

        /// <summary>
        /// Инициализирует сервис с валидатором строительства.
        /// </summary>
        /// <param name="validator">Сервис проверки возможности строительства.</param>
        public MapObjectPlacementService(ConstructionValidator validator)
        {
            _validator = validator;
        }

        /// <summary>
        /// Проверяет, можно ли разместить объект на указанной области карты.
        /// </summary>
        /// <param name="map">Карта для проверки.</param>
        /// <param name="mapObject">Объект, который требуется разместить.</param>
        /// <param name="area">Площадь, на которую планируется размещение.</param>
        /// <returns>True, если объект можно разместить, иначе false.</returns>
        public bool CanPlace(MapModel map, MapObject mapObject, Placement area)
        {
            if (!map.IsAreaInBounds(area))
                return false;

            foreach (var position in area.GetAllPositions())
            {
                TileModel tile = map[position];
                if (!_validator.CanBuildOnTile(tile, mapObject))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Пытается разместить объект на карте.
        /// </summary>
        /// <param name="map">Карта для размещения.</param>
        /// <param name="mapObject">Объект для размещения.</param>
        /// <param name="area">Область, куда объект будет размещен.</param>
        /// <returns>True, если размещение успешно, иначе false.</returns>
        public bool TryPlace(MapModel map, MapObject mapObject, Placement area)
        {
            if (!CanPlace(map, mapObject, area))
                return false;

            return map.TrySetMapObject(mapObject, area);
        }

        /// <summary>
        /// Удаляет объект с указанной области карты.
        /// </summary>
        /// <param name="map">Карта, с которой удаляется объект.</param>
        /// <param name="area">Область, в которой находится объект.</param>
        /// <remarks>
        /// Метод пока не реализован. Требует удаления объекта с тайлов карты
        /// и, возможно, уведомления подписчиков симуляции.
        /// </remarks>
        public void RemoveBuilding(MapModel map, Placement area)
        {
            throw new NotImplementedException();
        }
    }
}
