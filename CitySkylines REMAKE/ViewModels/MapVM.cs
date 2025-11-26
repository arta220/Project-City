using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Map;
using Services;
using System.Collections.ObjectModel;
using System.Windows;
using Domain.Base;

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
        public ObservableCollection<TileVM> Tiles { get; set; }

        public MapVM(Simulation simulation)
        {
            _simulation = simulation;
            Tiles = new();
            InitializeTiles();
            CreateHumanAndHome();
        }

        private void CreateHumanAndHome()
        {
            var home = new ResidentialBuilding(1, 1, new Area(1, 1));

            _simulation.TryPlace(home, new Placement(new Position(25, 25), home.Area));

            var citizen = new Citizen();
            citizen.Home = home;
            citizen.Position = new Position(10, 10);
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
                        if (e.PropertyName == nameof(tileVM.IsMouseOver) && ((TileVM)s).IsMouseOver && _startRoadTile != null)
                        {
                            UpdateRoadPreview(tileVM);
                        }
                    };
                }
            }
        }
        
        public void StartRoadConstruction(TileVM startTile)
        {
            // Проверяем, что находимся в режиме строительства и что выбран именно Road
            if (CurrentMode == MapInteractionMode.Build && SelectedObject?.Model is Road)
            {
                // Устанавливаем начальный тайл
                _startRoadTile = startTile;
                // Очищаем и добавляем начальный тайл в список
                ClearRoadPreview();
                _tilesToBuildRoadOn.Add(_startRoadTile);
                _startRoadTile.IsPreviewTile = true;
            }
        }

        // ОБНОВЛЕНИЕ ПРЕВЬЮ (при TileEnter / MouseMove)
        public void UpdateRoadPreview(TileVM currentTile)
        {
            // Если строительство дороги начато и это не тот же самый тайл
            if (_startRoadTile != null && currentTile != _startRoadTile)
            {
                // Очищаем старое превью, кроме стартового тайла
                ClearRoadPreview(keepStartTile: true); 

                // Вычисляем тайлы по прямой линии
                _tilesToBuildRoadOn = GetTilesAlongLine(_startRoadTile, currentTile);
                
                // Подсвечиваем все вычисленные тайлы
                foreach (var tile in _tilesToBuildRoadOn)
                {
                    // Проверяем возможность строительства
                    if (tile.CanBuild)
                    {
                        tile.IsPreviewTile = true;
                    }
                    else
                    {
                        // Если на пути есть занятый тайл, то дорога не строится
                        // (можно добавить более сложную логику, но пока так)
                        ClearRoadPreview(keepStartTile: true); 
                        break;
                    }
                }
            }
        }
        
        // ЗАВЕРШЕНИЕ СТРОИТЕЛЬСТВА (при TileClick/MouseUp)
        public void FinishRoadConstruction(TileVM endTile)
        {
            // Если строительство дороги было начато
            if (_startRoadTile != null && _tilesToBuildRoadOn.Count > 0)
            {
                // Сначала убедимся, что все выбранные тайлы пригодны для строительства
                bool canBuildAll = true;
                foreach(var tile in _tilesToBuildRoadOn)
                {
                    if (!tile.CanBuild)
                    {
                        canBuildAll = false;
                        break;
                    }
                }
                
                if (canBuildAll)
                {
                    var roadModel = SelectedObject.Model as Road;
                    
                    // Для каждого тайла в списке строим дорогу
                    foreach (var tile in _tilesToBuildRoadOn)
                    {
                        // Создаем новую Road-модель для каждого тайла, 
                        // так как Area дороги - это 1x1 тайл.
                        var singleTileRoad = new Road(new Area(1, 1)); 
                        var placement = new Placement(new Position(tile.X, tile.Y), singleTileRoad.Area);

                        // Пытаемся разместить. (RoadModel - это 1x1, поэтому просто ставим)
                        // ВАЖНО: Здесь нужно создать НОВЫЙ объект Road для каждого тайла,
                        // потому что каждый тайл будет иметь свою ссылку на MapObject.
                        _simulation.TryPlace(singleTileRoad, placement);
                    }

                    MessageBox.Show($"ПОСТРОЕНА ДОРОГА НА {_tilesToBuildRoadOn.Count} ТАЙЛАХ ОТЛАДКА");
                }
                else
                {
                    MessageBox.Show("НЕВОЗМОЖНО ПОСТРОИТЬ ДОРОГУ, ПУТЬ ЗАНЯТ ОТЛАДКА");
                }
                
                // Сбрасываем состояние
                ClearRoadPreview();
                _startRoadTile = null;
                _tilesToBuildRoadOn.Clear();
                CurrentMode = MapInteractionMode.None;
            }
        }
        
        // Вспомогательный метод для очистки превью
        private void ClearRoadPreview(bool keepStartTile = false)
        {
            foreach (var tile in _tilesToBuildRoadOn)
            {
                if (keepStartTile && tile == _startRoadTile) continue;
                tile.IsPreviewTile = false;
            }
            _tilesToBuildRoadOn.Clear();
            if(keepStartTile && _startRoadTile != null)
                _tilesToBuildRoadOn.Add(_startRoadTile);
        }
        
        // Реализация Алгоритма Брезенхема для получения тайлов по прямой линии
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
                // Находим тайл по координатам
                // Предполагается, что Tiles - это одномерный список, 
                // и их нужно найти по координатам (X, Y)
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
                return; // Завершили строительство дороги и выходим
            }
            
            switch (CurrentMode)
            {
                case MapInteractionMode.Build:
                    if (SelectedObject != null)
                    {
                        var building = SelectedObject.Model;
                        var placement = new Placement(new Position(tile.X, tile.Y), building.Area);

                        if (_simulation.TryPlace(building, placement))
                        {
                            MessageBox.Show("ЗДАНИЕ ПОСТАВЛЕНО ОТЛАДКА");
                        }
                        else
                        {
                            MessageBox.Show("НЕВОЗМОЖНО ПОСТРОИТЬ ЗДАНИЕ ОТЛАДКА");
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