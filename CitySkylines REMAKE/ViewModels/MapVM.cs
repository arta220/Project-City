using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Map;
using System.Collections.ObjectModel;
using System.Windows;
using Domain.Base;
using Services;
using System.Diagnostics;
using System.Windows.Input;

namespace CitySimulatorWPF.ViewModels
{
    public partial class MapVM : ObservableObject
    {
        [ObservableProperty]
        private ObjectVM _selectedObject;

        [ObservableProperty]
        private MapInteractionMode _currentMode = MapInteractionMode.None;

        private readonly Simulation _simulation;

        private TileVM _startRoadTile;
        private List<TileVM> _tilesToBuildRoadOn = new List<TileVM>();

        public int Width => _simulation.MapModel.Width;
        public int Height => _simulation.MapModel.Height;

        public ObservableCollection<TileVM> Tiles { get; }
        public ObservableCollection<CitizenVM> Citizens { get; }

        public MapVM(Simulation simulation)
        {
            _simulation = simulation;
            Tiles = new ObservableCollection<TileVM>();
            Citizens = new ObservableCollection<CitizenVM>();

            InitializeTiles();
            SubscribeToSimulationEvents();
            CreateHumanAndHome();
        }

        private void SubscribeToSimulationEvents()
        {
            _simulation.CitizenAdded += OnCitizenAdded;
            _simulation.CitizenRemoved += OnCitizenRemoved;
            _simulation.TickOccurred += OnSimulationTick;

            foreach (var citizen in _simulation.Citizens)
            {
                Citizens.Add(new CitizenVM(citizen));
            }
        }

        private void OnCitizenAdded(Citizen citizen)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                Citizens.Add(new CitizenVM(citizen));
            });
        }

        private void OnCitizenRemoved(Citizen citizen)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                var citizenVM = Citizens.FirstOrDefault(c => c.Citizen == citizen);
                if (citizenVM != null)
                    Citizens.Remove(citizenVM);
            });
        }

        private void OnSimulationTick(int tick)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                foreach (var citizenVM in Citizens)
                {
                    citizenVM.UpdatePosition();
                }
            });
        }

        private void CreateHumanAndHome()
        {
            var home = new ResidentialBuilding(1, 1, new Area(1, 1));
            _simulation.TryPlace(home, new Placement(new Position(25, 25), home.Area));

            var citizen = new Citizen();
            citizen.Home = home;
            citizen.Position = new Position(0, 0);
            citizen.State = CitizenState.GoingHome;
            _simulation.AddCitizen(citizen);
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
                    tileVM.TileConstructionStart += StartRoadConstruction;

                    tileVM.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(tileVM.IsMouseOver) &&
                            ((TileVM)s).IsMouseOver && _startRoadTile != null)
                        {
                            UpdateRoadPreview(tileVM);
                        }
                    };
                }
            }
        }

        public void StartRoadConstruction(TileVM startTile)
        {
            if (CurrentMode == MapInteractionMode.Build && SelectedObject?.Model is Road)
            {
                _startRoadTile = startTile;
                ClearRoadPreview();
                _tilesToBuildRoadOn.Add(_startRoadTile);
                _startRoadTile.IsPreviewTile = true;
            }
        }

        public void UpdateRoadPreview(TileVM currentTile)
        {
            if (_startRoadTile != null && currentTile != _startRoadTile)
            {
                ClearRoadPreview(keepStartTile: true);
                _tilesToBuildRoadOn = GetTilesAlongLine(_startRoadTile, currentTile);

                foreach (var tile in _tilesToBuildRoadOn)
                {
                    if (tile.CanBuild)
                    {
                        tile.IsPreviewTile = true;
                    }
                    else
                    {
                        ClearRoadPreview(keepStartTile: true);
                        break;
                    }
                }
            }
        }

        public void FinishRoadConstruction(TileVM endTile)
        {
            if (_startRoadTile != null && _tilesToBuildRoadOn.Count > 0)
            {
                bool canBuildAll = true;
                foreach (var tile in _tilesToBuildRoadOn)
                {
                    if (!tile.CanBuild)
                    {
                        canBuildAll = false;
                        break;
                    }
                }

                if (canBuildAll)
                {
                    foreach (var tile in _tilesToBuildRoadOn)
                    {
                        var singleTileRoad = new Road(new Area(1, 1));
                        var placement = new Placement(new Position(tile.X, tile.Y), singleTileRoad.Area);
                        _simulation.TryPlace(singleTileRoad, placement);
                    }
                }
                else
                {

                }

                ClearRoadPreview();
                _startRoadTile = null;
                _tilesToBuildRoadOn.Clear();
                CurrentMode = MapInteractionMode.None;
            }
        }

        private void ClearRoadPreview(bool keepStartTile = false)
        {
            foreach (var tile in _tilesToBuildRoadOn)
            {
                if (keepStartTile && tile == _startRoadTile) continue;
                tile.IsPreviewTile = false;
            }
            _tilesToBuildRoadOn.Clear();
            if (keepStartTile && _startRoadTile != null)
                _tilesToBuildRoadOn.Add(_startRoadTile);
        }

        private List<TileVM> GetTilesAlongLine(TileVM start, TileVM end)
        {
            var lineTiles = new List<TileVM>();

            int x0 = start.X;
            int y0 = start.Y;
            int x1 = end.X;
            int y1 = end.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                TileVM currentTile = Tiles.FirstOrDefault(t => t.X == x0 && t.Y == y0);

                if (currentTile != null)
                {
                    lineTiles.Add(currentTile);
                }

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return lineTiles;
        }

        public void OnTileClicked(TileVM tile)
        {
            if (_startRoadTile != null)
            {
                FinishRoadConstruction(tile);
                return;
            }

            switch (CurrentMode)
            {
                case MapInteractionMode.Build:
                    if (SelectedObject != null)
                    {
                        var building = SelectedObject.Model;
                        var placement = new Placement(new Position(tile.X, tile.Y), building.Area);

                        if (!_simulation.TryPlace(building, placement))
                            Debug.WriteLine("Невозможно поставить здание");

                        CurrentMode = MapInteractionMode.None;
                    }
                    break;
                case MapInteractionMode.Remove:
                    // Логика удаления объектов
                    break;
                case MapInteractionMode.None:
                    // Показ информации о клетке
                    break;
            }
        }

        public void Cleanup()
        {
            _simulation.CitizenAdded -= OnCitizenAdded;
            _simulation.CitizenRemoved -= OnCitizenRemoved;
            _simulation.TickOccurred -= OnSimulationTick;
        }
    }
}