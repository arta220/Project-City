using Core.Models.Base;
using System.Printing;

namespace CitySkylines_REMAKE.Models.Map
{
    // контейнер для тайлов карты
    public class MapModel
    {
        // сетка, дублирующая размеры карты
        // если здание есть - содержит ссылку на него
        // если здания нет - null
        public Building[,] _buildingsGrid;

        // ширина/высота карты
        public int Width { get; init; }
        public int Height { get; init; }

        // сама карта тайлов
        private TileModel[,] _tiles;

        public MapModel(int width, int height)
        {
            Width = width;
            Height = height;
            _tiles = new TileModel[Width, Height];
        }


        // крутая штука
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
        
        // метод, возвращающий тип здания на тайле
        // 
        public Building GetBuildingAt(int x, int y)
        {
            if (x >= 0 && x < Width &&
                y >= 0 && y < Height)
                return _buildingsGrid[x, y];
            return null;
        }
    }
}
