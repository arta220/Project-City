using Domain.Base;
using Domain.Common.Base.MovingEntities;
using Domain.Common.Enums;
using Domain.Map;
using Domain.Transports;

namespace Services.EntityMovement.Profile
{
    /// <summary>
    /// Навигационный профиль для транспорта.
    /// Определяет проходимость и стоимость прохождения тайлов для транспортных средств.
    /// </summary>
    public class TransportProfile : INavigationProfile
    {
        private readonly MapModel _mapModel;
        private readonly TransportType _transportType;

        // Характеристики транспорта
        public float MaxSpeed { get; } // км/ч
        public float TurningRadius { get; } // клетки

        public TransportProfile(MapModel mapModel, TransportType transportType = TransportType.Car)
        {
            _mapModel = mapModel;
            _transportType = transportType;

            // Настраиваем характеристики по типу транспорта
            switch (transportType)
            {
                case TransportType.Car:
                    MaxSpeed = 60f;
                    TurningRadius = 2f;
                    break;
                case TransportType.Bus:
                    MaxSpeed = 40f;
                    TurningRadius = 3f;
                    break;
                case TransportType.Taxi:
                    MaxSpeed = 60f;
                    TurningRadius = 2f;
                    break;
            }
        }

        /// <summary>
        /// Проверяет, может ли транспорт войти на указанную позицию
        /// </summary>
        public bool CanEnter(Position pos)
        {
            if (!_mapModel.IsPositionInBounds(pos))
                return false;

            var tile = _mapModel[pos];

            // Вода и горы непроходимы для любого транспорта
            if (tile.Terrain == TerrainType.Water || tile.Terrain == TerrainType.Mountain)
                return false;

            // Если на тайле есть объект
            if (tile.MapObject != null)
            {
                // Дороги проходимы
                if (tile.MapObject is Road)
                    return true;

                // Пути могут быть проходимы для некоторых типов транспорта
                if (tile.MapObject is Domain.Infrastructure.Path path)
                {
                    // Например, велосипедные дорожки для автомобилей не проходимы
                    if (_transportType == TransportType.Car && path.Type == PathType.Bicycle)
                        return false;

                    // Пешеходные дорожки для транспорта не проходимы
                    if (path.Type == PathType.Pedestrian)
                        return false;
                }

                // Все остальные объекты (здания) непроходимы
                return false;
            }

            // Пустой тайл: проверяем тип местности
            switch (tile.Terrain)
            {
                case TerrainType.Forest:
                    // Лес проходим, но с высокой стоимостью
                    return true;
                case TerrainType.Meadow:
                case TerrainType.Plain:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Возвращает стоимость прохождения тайла
        /// </summary>
        public int GetTileCost(Position pos)
        {
            if (!_mapModel.IsPositionInBounds(pos))
                return int.MaxValue;

            var tile = _mapModel[pos];
            int baseCost = 10;

            // Дороги - самый дешевый вариант
            if (tile.MapObject is Road road)
                baseCost = 5;
            else if (tile.MapObject is Domain.Infrastructure.Path)
                // Пути нежелательны для транспорта
                baseCost = 20;

            // Модификаторы от типа местности
            switch (tile.Terrain)
            {
                case TerrainType.Plain:
                    baseCost += 5;
                    break;
                case TerrainType.Forest:
                    baseCost += 30; // Лес - очень высокая стоимость
                    break;
                case TerrainType.Meadow:
                    baseCost += 10;
                    break;
            }

            // Учитываем высоту (подъем/спуск)
            float height = tile.Height;
            if (height > 0.5f) // Крутой подъем
                baseCost += (int)(height * 15);

            return baseCost;
        }

        /// <summary>
        /// Проверяет, может ли транспорт развернуться в данной позиции
        /// </summary>
        public bool CanTurnAround(Position pos)
        {
            if (!_mapModel.IsPositionInBounds(pos))
                return false;

            // Проверяем достаточно ли места для поворота
            var radius = (int)Math.Ceiling(TurningRadius);
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    var checkPos = new Position(pos.X + dx, pos.Y + dy);
                    if (_mapModel.IsPositionInBounds(checkPos) && !CanEnter(checkPos))
                        return false;
                }
            }
            return true;
        }
    }
}


