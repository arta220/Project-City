using Domain.Common.Base;
using Domain.Map;

namespace Services.PlaceBuilding
{
    /// <summary>
    /// Сервис проверки возможности строительства объектов на тайлах карты.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется сервисом <see cref="MapObjectPlacementService"/> для проверки,
    ///   можно ли разместить объект на конкретном тайле.
    /// - В данном проекте проверка ограничена методом <see cref="TileModel.CanPlace"/>,
    ///   но сервис можно расширить для более сложных правил (например, проверка соседних тайлов,
    ///   типа местности, ограничений по зданиям и др.).
    /// </remarks>
    public class ConstructionValidator
    {
        /// <summary>
        /// Определяет, можно ли разместить объект на указанном тайле.
        /// </summary>
        /// <param name="tile">Тайл карты для проверки.</param>
        /// <param name="mapObject">Объект, который нужно разместить.</param>
        /// <returns>True, если объект можно разместить, иначе false.</returns>
        public bool CanBuildOnTile(TileModel tile, MapObject mapObject)
            => tile.CanPlace(mapObject);
    }
}
