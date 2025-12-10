using Services.Interfaces;
using Domain.Map;
using Domain.Common.Base;
using Domain.Base;
using Domain.Infrastructure;
using Domain.Common.Enums;

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

            // Дороги и пути могут размещаться рядом друг с другом
            bool isInfrastructure = mapObject is Road || mapObject is Domain.Infrastructure.Path;

            foreach (var position in area.GetAllPositions())
            {
                TileModel tile = map[position];
                
                // Для инфраструктуры (дороги, пути) разрешаем размещение на тайлах с другой инфраструктурой
                // Для зданий - используем стандартную проверку
                if (isInfrastructure)
                {
                    // Дороги и пути могут размещаться на тайлах, где уже есть другая инфраструктура
                    // (например, пересечение дорог), но не могут размещаться на зданиях или воде/горах
                    if (tile.MapObject != null && !(tile.MapObject is Road || tile.MapObject is Domain.Infrastructure.Path))
                        return false;
                    
                    // Проверяем рельеф (вода и горы непроходимы)
                    if (tile.Terrain == TerrainType.Water || tile.Terrain == TerrainType.Mountain)
                        return false;
                }
                else
                {
                    // Для зданий используем стандартную проверку
                    if (!_validator.CanBuildOnTile(tile, mapObject))
                        return false;
                    
                    // Здания не могут размещаться рядом с другими объектами
                    foreach (var neighbor in position.GetNeighbors())
                    {
                        if (!map.IsPositionInBounds(neighbor))
                            continue;

                        var neighborObject = map[neighbor].MapObject;
                        if (neighborObject != null)
                            return false;
                    }
                }
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
        public bool TryRemove(MapModel map, Placement area)
        {
            if (map.TryRemoveMapObject(area))
                return true;

            return false;
        }

    }
}
