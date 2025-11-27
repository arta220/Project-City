using CitySimulatorWPF.Services;
using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Base;
using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Map;
using Services;
using Services.CitizensSimulation;
using System.Collections.ObjectModel;

namespace CitySimulatorWPF.ViewModels
{
    public partial class MapVM : ObservableObject
    {
        [ObservableProperty]
        private ObjectVM _selectedObject;

        [ObservableProperty]
        private MapInteractionMode _currentMode = MapInteractionMode.None;

        private readonly Simulation _simulation;
        private readonly IRoadConstructionService _roadService;
        private readonly ICitizenManagerService _citizenManager;
        private readonly IMapTileService _mapTileService;
        private readonly MessageService _messageService;
        private readonly CitizenSimulationService _citizenSimulation;

        public ObservableCollection<TileVM> Tiles => _mapTileService.Tiles;
        public ObservableCollection<CitizenVM> Citizens => _citizenManager.Citizens;

        public int Width => _simulation.MapModel.Width;
        public int Height => _simulation.MapModel.Height;

        public MapVM(Simulation simulation,
                     IRoadConstructionService roadService,
                     ICitizenManagerService citizenManager,
                     IMapTileService mapTileService,
                     MessageService messageService,
                     CitizenSimulationService citizenSimulation)
        {
            _simulation = simulation;
            _roadService = roadService;
            _citizenManager = citizenManager;
            _mapTileService = mapTileService;
            _messageService = messageService;
            _citizenSimulation = citizenSimulation;

            _citizenManager.StartSimulation(_citizenSimulation);

            _citizenSimulation.Start();

            _mapTileService.InitializeTiles(
                _simulation.MapModel,
                onTileClicked: OnTileClicked,
                onTileConstructionStart: OnTileConstructionStart,
                onMouseOverPreview: tile =>
                {
                    if (_roadService.IsBuilding)
                        _roadService.UpdatePreview(tile);
                    return true;
                });

            CreateHumanAndHome();
        }


        private void CreateHumanAndHome()
        {
            var home = new ResidentialBuilding(1, 1, new Area(1, 1));
            _simulation.TryPlace(home, new Placement(new Position(25, 25), home.Area));

            var citizen = new Citizen
            {
                Home = home,
                Position = new Position(0, 0),
                State = CitizenState.GoingHome
            };

            _citizenSimulation.AddCitizen(citizen);
        }

        private void OnTileConstructionStart(TileVM tile)
        {
            if (CurrentMode == MapInteractionMode.Build && SelectedObject?.Model is Road)
            {
                _roadService.StartConstruction(tile);
            }
        }

        private void OnTileClicked(TileVM tile)
        {
            if (_roadService.IsBuilding)
            {
                _roadService.FinishConstruction(tile, (road, placement) => _simulation.TryPlace(road, placement));
                CurrentMode = MapInteractionMode.None;
                return;
            }

            if (CurrentMode == MapInteractionMode.Build && SelectedObject?.Model is Building building)
            {
                var placement = new Placement(new Position(tile.X, tile.Y), building.Area);
                if (!_simulation.TryPlace(building, placement))
                {
                    _messageService.ShowMessage("Невозможно поставить здание");
                }
                CurrentMode = MapInteractionMode.None;
                return;
            }

            if (CurrentMode == MapInteractionMode.Remove)
            {
                return;
            }

            if (CurrentMode == MapInteractionMode.None)
            {

            }
        }

        public void Cleanup()
        {
            _citizenManager.StopSimulation();
            _citizenSimulation.Stop();
        }

    }
}
