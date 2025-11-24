using Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace CitySimulatorWPF.ViewModels
{
    public partial class MapVM : ObservableObject
    {
        private readonly Simulation _simulation;

        public int Width => _simulation.MapModel.Width;
        public int Height => _simulation.MapModel.Height;
        public ObservableCollection<TileVM> Tiles { get; set; }

        public MapVM(Simulation simulation)
        {
            _simulation = simulation;
            Tiles = new();
            InitializeTiles();

        }

        private void InitializeTiles()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var tileVM = new TileVM(_simulation.MapModel[x, y]);
                    Tiles.Add(tileVM);

                    tileVM.TileClicked += OnTileClicked;
                }
            }
        }

        public void OnTileClicked(TileVM tile)
        {
            MessageBox.Show($"Клик по тайлу {tile.X}, {tile.Y}");
        }
    }
}