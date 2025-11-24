using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Map;
using Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace CitySimulatorWPF.ViewModels
{
    public partial class MapVM : ObservableObject
    {
        [ObservableProperty]
        private BuildingVM _selectedBuilding;

        [ObservableProperty]
        private MapInteractionMode _currentMode = MapInteractionMode.None;

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
            switch (CurrentMode)
            {
                case MapInteractionMode.Build:
                    if (SelectedBuilding != null)
                    {
                        var building = SelectedBuilding.Model;
                        var placement = new Placement(new Position(tile.X, tile.Y), building.Area);

                        if (_simulation.TryPlaceBuilding(building, placement))
                        {
                            // Ура
                        }
                        else
                        {
                            // Уведомление о невозможности постройки
                        }
                        CurrentMode = MapInteractionMode.None;
                    }
                    break;
                case MapInteractionMode.Remove:
                    break;
                case MapInteractionMode.None:
                    // Можно добавить сервис информационный, показывать информацию о клетке.
                    break;
                default:
                    break;
            }
        }
    }
}