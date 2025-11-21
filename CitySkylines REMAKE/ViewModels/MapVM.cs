using CitySkylines_REMAKE.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class MapVM : ObservableObject
    {
        private readonly MapModel _mapModel;

        public int Width => _mapModel.Width;
        public int Height => _mapModel.Height;
        public ObservableCollection<TileVM> Tiles { get; set; }

        public MapVM()
        {
            _mapModel = new MapModel(25, 25);
            Tiles = new();
            InitializeTiles();
        }

        private void InitializeTiles()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var tileVM = new TileVM(_mapModel[x, y]);
                    Tiles.Add(tileVM);
                }
            }
        }
    }
}
