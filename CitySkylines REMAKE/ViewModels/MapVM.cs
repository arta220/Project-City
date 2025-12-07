using CitySimulatorWPF.Services;
using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Buildings;
using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Domain.Transports.Ground;
using Domain.Transports.States;
using System.Linq;
using Services;
using Services.Citizens.Job.Movement;
using Services.Citizens.Scenaries;
using Services.Citizens.Scenarios;
using Services.CitizensSimulation;
using Services.CitizensSimulation.CitizenSchedule;
using Services.TransportSimulation;
using Services.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Windows.Threading;

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
        private readonly ICarManagerService _carManager;
        private readonly IMapTileService _mapTileService;
        private readonly MessageService _messageService;
        private readonly IUtilityService _utilityService;
        private readonly IPathConstructionService _pathService;

        private bool _simulationStarted = false;

        public ObservableCollection<TileVM> Tiles => _mapTileService.Tiles;
        public ObservableCollection<CitizenVM> Citizens => _citizenManager.Citizens;
        public ObservableCollection<PersonalCarVM> Cars => _carManager.Cars;

        // Иконки зданий для отдельного слоя поверх тайлов
        public ObservableCollection<BuildingIconVM> BuildingIcons { get; } = new();

        public int Width => _simulation.MapModel.Width;
        public int Height => _simulation.MapModel.Height;

        public MapVM(Simulation simulation,
                     IRoadConstructionService roadService,
                     ICitizenManagerService citizenManager,
                     ICarManagerService carManager,
                     IMapTileService mapTileService,
                     MessageService messageService,
                     CitizenSimulationService citizenSimulation,
                     TransportSimulationService transportSimulation,
                     IUtilityService utilityService,
                     IPathConstructionService pathService)
        {
            _simulation = simulation;
            _roadService = roadService;
            _citizenManager = citizenManager;
            _carManager = carManager;
            _mapTileService = mapTileService;
            _messageService = messageService;
            _utilityService = utilityService;
            _pathService = pathService;

            _citizenManager.StartSimulation(citizenSimulation);
            _carManager.StartSimulation(transportSimulation);

            _mapTileService.InitializeTiles(
                _simulation.MapModel,
                onTileClicked: OnTileClicked,
                onTileConstructionStart: OnTileConstructionStart,
                onMouseOverPreview: tile =>
                {
                    if (_roadService.IsBuilding) _roadService.UpdatePreview(tile);
                    if (_pathService.IsBuilding) _pathService.UpdatePreview(tile);
                    return true;
                });


            // Подписка на событие размещения/удаления объектов, чтобы управлять крупными иконками зданий
            _simulation.MapObjectPlaced  += OnMapObjectPlaced;
            _simulation.MapObjectRemoved += OnMapObjectRemoved;

            // CreateTestScenarioCardboard(); Тестирование фабрики картона и фабрики упаковки
            CreateTestScenario();


            CreateTestScenario();
            StartSimulationAfterUIReady();
        }

        private void OnMapObjectPlaced(MapObject mapObject)
        {
            var (placement, found) = _simulation.GetMapObjectPlacement(mapObject);
            if (!found || placement is null)
                return;

            const int tileSize = 20; // как в CitizenVM / PersonalCarVM

            var iconVm = new BuildingIconVM(mapObject, (Placement)placement, tileSize);
            BuildingIcons.Add(iconVm);
        }

        private void OnMapObjectRemoved(MapObject mapObject)
        {
            var icon = BuildingIcons.FirstOrDefault(b => ReferenceEquals(b.MapObject, mapObject));
            if (icon != null)
                BuildingIcons.Remove(icon);
        }

        private void StartSimulationAfterUIReady()
        {
            if (_simulationStarted) return;

            _simulationStarted = true;
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                _citizenManager.ResumeSimulation();
                _carManager.ResumeSimulation();
            }, DispatcherPriority.Background);
        }

        private void CreateTestScenario()
        {
            // 1. Создаём жителя (работника ЖКХ)
            var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
            {
                Age = 30,
                Profession = CitizenProfession.UtilityWorker,
                State = CitizenState.Idle
            };
            citizen.Position = new Position(15, 15);
            _simulation.AddCitizen(citizen);
            Debug.WriteLine($"Создан работник ЖКХ ID: {citizen.Id} на позиции ({citizen.Position.X}, {citizen.Position.Y})");

            // 2. Создаём офис ЖКХ
            var utilityOfficeFactory = new UtilityOfficeFactory();
            var utilityOffice = (Building)utilityOfficeFactory.Create();
            var officePlacement = new Placement(new Position(25, 25), utilityOffice.Area);
            if (!_simulation.TryPlace(utilityOffice, officePlacement))
            {
                _messageService.ShowMessage("Не удалось разместить офис ЖКХ");
                return;
            }
            citizen.WorkPlace = utilityOffice;
            Debug.WriteLine($"Создан офис ЖКХ на позиции (25,25). Назначен как WorkPlace работнику {citizen.Id}");

            // 3. Создаём тестовый жилой дом
            var residentialFactory = new SmallHouseFactory();
            var residentialBuilding = (ResidentialBuilding)residentialFactory.Create();
            var housePlacement = new Placement(new Position(35, 35), residentialBuilding.Area);
            if (!_simulation.TryPlace(residentialBuilding, housePlacement))
            {
                _messageService.ShowMessage("Не удалось разместить жилой дом");
                return;
            }
            Debug.WriteLine($"Создан жилой дом на позиции (35,35)");

            // 4. Ломаем коммуналку для теста
            _utilityService.BreakUtilityForTesting(residentialBuilding, UtilityType.Electricity, currentTick: 1);
            var brokenUtilities = _utilityService.GetBrokenUtilities(residentialBuilding);
            Debug.WriteLine($"Сломанные коммуналки в тестовом доме: {brokenUtilities.Count}");

            // 7. Информация о тесте
            _messageService.ShowMessage(
                "Тестовый сценарий создан!\n" +
                "1. Работник ЖКХ: (15,15)\n" +
                "2. Офис ЖКХ: (25,25)\n" +
                "3. Жилой дом: (35,35) - СЛОМАНО ЭЛЕКТРИЧЕСТВО\n\n" +
                "Работник должен побежать чинить сломанное ЖКХ."
            );
        }




        private void OnTileConstructionStart(TileVM tile)
        {
            if (SelectedObject?.Factory is IRoadFactory)
                _roadService.StartConstruction(tile);
            else if (SelectedObject?.Factory is PedestrianPathFactory)
                _pathService.StartConstruction(tile, PathType.Pedestrian);
            else if (SelectedObject?.Factory is BicyclePathFactory)
                _pathService.StartConstruction(tile, PathType.Bicycle);
        }

        private void OnTileClicked(TileVM tile)
        {
            if (_roadService.IsBuilding)
            {
                _roadService.FinishConstruction(tile, (road, placement) => _simulation.TryPlace(road, placement));
                CurrentMode = MapInteractionMode.None;
                return;
            }

            if (_pathService.IsBuilding)
            {
                _pathService.FinishConstruction(tile, (path, placement) => _simulation.TryPlace(path, placement));
                CurrentMode = MapInteractionMode.None;
                return;
            }

            if (CurrentMode == MapInteractionMode.Build && SelectedObject != null)
            {
                var obj = SelectedObject.Factory.Create();
                var placement = new Placement(new Position(tile.X, tile.Y), obj.Area);

                if (!_simulation.TryPlace(obj, placement))
                {
                    _messageService.ShowMessage("Невозможно поставить объект");
                }
                else
                {
                    // Левый верхний тайл здания — якорный, на нём и показываем иконку
                    tile.IsMainObjectTile = true;
                }

                CurrentMode = MapInteractionMode.None;
                return;
            }

            if (CurrentMode == MapInteractionMode.None && tile.MapObject is ResidentialBuilding residentialBuilding)
            {
                if (residentialBuilding.Utilities.HasBrokenUtilities)
                    ShowRepairDialog(residentialBuilding, tile);
            }

            if (CurrentMode == MapInteractionMode.Remove)
                _simulation.TryRemove(tile.MapObject);
        }

        private void ShowRepairDialog(ResidentialBuilding building, TileVM tile)
        {
            var brokenUtilities = _utilityService.GetBrokenUtilities(building);
            if (!brokenUtilities.Any())
            {
                _messageService.ShowMessage("Нет сломанных коммунальных услуг");
                return;
            }

            string message = "Что починить?\n";
            int i = 1;
            var utilitiesList = brokenUtilities.Keys.ToList();
            foreach (var utility in utilitiesList)
            {
                message += $"{i}. {utility} - сломано с тика {brokenUtilities[utility]}\n";
                i++;
            }
            message += "\nВведите номер (или 0 для отмены):";

            string input = Microsoft.VisualBasic.Interaction.InputBox(message, "Ремонт коммуналки", "0");

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= utilitiesList.Count)
            {
                var utilityToFix = utilitiesList[choice - 1];
                _utilityService.FixUtility(building, utilityToFix);
                tile.UpdateBlinkingState();
                _messageService.ShowMessage($"{utilityToFix} отремонтирован!");
            }
        }
    }
}
