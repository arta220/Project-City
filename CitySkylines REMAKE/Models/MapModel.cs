using System.Printing;

namespace CitySkylines_REMAKE.Models
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
    }
}
