using Domain.Base;

namespace Domain.Map
{
    public class MapModel
    {
        public int Width { get; init; }
        public int Height { get; init; }

        private TileModel[,] _tiles;

        public MapModel(int width, int height)
        {
            Width = width;
            Height = height;
            _tiles = new TileModel[Width, Height];
        }

        public TileModel this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height) throw new ArgumentOutOfRangeException("Индекс за пределами доски.");
                return _tiles[x, y];
            }
            set
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height) throw new ArgumentOutOfRangeException("Индекс за пределами доски.");
                _tiles[x, y] = value;
            }
        }

        public TileModel this[Position position]
        {
            get => _tiles[position.X, position.Y];
            set => _tiles[position.X, position.Y] = value;
        }

        public bool TrySetBuilding(Building building, Area area)
        {
            foreach (var pos in area.GetAllPositions())
                _tiles[pos.X, pos.Y].Building = building;

            return true;
        }

        public bool TryRemoveBuilding(Area area)
        {
            foreach (var pos in area.GetAllPositions())
                _tiles[pos.X, pos.Y].Building = null;

            return true;
        }

        public bool IsAreaInBounds(Area area)
        {
            return IsPositionInBounds(area.Position) &&
                   IsPositionInBounds(new Position(area.Right - 1, area.Bottom - 1));
        }

        public bool IsPositionInBounds(Position position) => position.X < Width && position.Y < Height &&
                                                             position.X >= 0 && position.Y >= 0;
    }
}
