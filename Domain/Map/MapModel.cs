using Domain.Base;

namespace Domain.Map
{
    /// <summary>
    /// Представляет карту с сеткой плиток и размещёнными на ней объектами.
    /// </summary>
    public class MapModel
    {
        /// <summary>
        /// Ширина карты в плитках.
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// Высота карты в плитках.
        /// </summary>
        public int Height { get; init; }

        private TileModel[,] _tiles;

        /// <summary>
        /// Создаёт новую карту с заданными размерами.
        /// </summary>
        /// <param name="width">Ширина карты в плитках.</param>
        /// <param name="height">Высота карты в плитках.</param>
        public MapModel(int width, int height)
        {
            Width = width;
            Height = height;
            _tiles = new TileModel[Width, Height];
        }

        /// <summary>
        /// Индексатор для доступа к плиткам по координатам X и Y.
        /// </summary>
        /// <param name="x">Координата X плитки.</param>
        /// <param name="y">Координата Y плитки.</param>
        /// <returns>Модель плитки <see cref="TileModel"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Если координаты выходят за пределы карты.</exception>
        public TileModel this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    throw new ArgumentOutOfRangeException("Индекс за пределами карты.");
                return _tiles[x, y];
            }
            set
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    throw new ArgumentOutOfRangeException("Индекс за пределами карты.");
                _tiles[x, y] = value;
            }
        }

        /// <summary>
        /// Индексатор для доступа к плитке по позиции <see cref="Position"/>.
        /// </summary>
        /// <param name="position">Позиция плитки.</param>
        /// <returns>Модель плитки <see cref="TileModel"/>.</returns>
        public TileModel this[Position position]
        {
            get => _tiles[position.X, position.Y];
            set => _tiles[position.X, position.Y] = value;
        }

        /// <summary>
        /// Пытается разместить объект на карте в указанной области.
        /// </summary>
        /// <param name="mapObject">Объект карты для размещения.</param>
        /// <param name="area">Площадь размещения объекта.</param>
        /// <returns>Всегда возвращает true (пока нет проверок на пересечение).</returns>
        /// <remarks>
        /// Можно расширить для проверки на коллизии и доступность плиток.
        /// </remarks>
        public bool TrySetMapObject(MapObject mapObject, Placement area)
        {
            foreach (var pos in area.GetAllPositions())
            {
                _tiles[pos.X, pos.Y].MapObject = mapObject;
            }

            return true;
        }

        /// <summary>
        /// Удаляет объект с карты в указанной области.
        /// </summary>
        /// <param name="area">Область, с которой удаляется объект.</param>
        /// <returns>Всегда возвращает true.</returns>
        public bool TryRemoveMapObject(Placement area)
        {
            foreach (var pos in area.GetAllPositions())
                _tiles[pos.X, pos.Y].MapObject = null;

            return true;
        }

        /// <summary>
        /// Проверяет, полностью ли область находится в пределах карты.
        /// </summary>
        /// <param name="area">Область для проверки.</param>
        /// <returns>True, если область целиком находится внутри карты.</returns>
        public bool IsAreaInBounds(Placement area)
        {
            return IsPositionInBounds(area.Position) &&
                   IsPositionInBounds(new Position(area.Right - 1, area.Bottom - 1));
        }

        /// <summary>
        /// Проверяет, находится ли заданная позиция в пределах карты.
        /// </summary>
        /// <param name="position">Позиция для проверки.</param>
        /// <returns>True, если позиция внутри карты.</returns>
        public bool IsPositionInBounds(Position position) =>
            position.X >= 0 && position.Y >= 0 && position.X < Width && position.Y < Height;
    }
}
