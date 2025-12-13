using CitySimulatorWPF.Services;
using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Buildings;
using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Base.MovingEntities;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.CitizensSimulation;
using Services.Disasters;
using Services.Factories;
using Services.TransportSimulation;
using Services.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace CitySimulatorWPF.ViewModels
{
    public partial class MapVM : ObservableObject
    {
        [ObservableProperty]
        private ObjectVM _selectedObject;
        private readonly CitizenFactory _citizenFactory;
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
        private readonly IDisasterService _disasterService;

        private bool _simulationStarted = false;

        // Поля для отслеживания двойного клика
        private TileVM _lastClickedTile;
        private DateTime _lastTileClickTime = DateTime.MinValue;

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
                     IPathConstructionService pathService,
                     IDisasterService disasterService,
                     CitizenFactory citizenFactory)
        {
            _simulation = simulation;
            _roadService = roadService;
            _citizenManager = citizenManager;
            _carManager = carManager;
            _mapTileService = mapTileService;
            _messageService = messageService;
            _utilityService = utilityService;
            _pathService = pathService;
            _disasterService = disasterService;
            _citizenFactory = citizenFactory;
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
            _simulation.MapObjectPlaced += OnMapObjectPlaced;
            _simulation.MapObjectRemoved += OnMapObjectRemoved;

            // CreateTestScenarioCardboard(); Тестирование фабрики картона и фабрики упаковки

            CreateTestJobScenario();

            //CreateTestScenario();

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
            // Удаление должно происходить в UI потоке
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Ищем иконку по ссылке на объект
                BuildingIconVM iconToRemove = null;
                foreach (var icon in BuildingIcons)
                {
                    if (ReferenceEquals(icon.MapObject, mapObject))
                    {
                        iconToRemove = icon;
                        break;
                    }
                }

                if (iconToRemove != null)
                {
                    BuildingIcons.Remove(iconToRemove);
                    System.Diagnostics.Debug.WriteLine($"[MapVM] Successfully removed icon for building of type {mapObject?.GetType().Name ?? "null"}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[MapVM] WARNING: Icon not found for building of type {mapObject?.GetType().Name ?? "null"}, total icons: {BuildingIcons.Count}");
                    // Выводим список всех иконок для отладки
                    foreach (var icon in BuildingIcons)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MapVM] Icon exists for {icon.MapObject?.GetType().Name ?? "null"} object");
                    }
                }
            });
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
            var citizen = _citizenFactory.CreateCitizen(
                pos: new Position(15, 15),
                speed: 1.0f,
                profession: CitizenProfession.UtilityWorker
            );
            _simulation.AddCitizen(citizen);
            Debug.WriteLine($"Создан работник ЖКХ ID: {citizen.Id} на позиции ({citizen.Position.X}, {citizen.Position.Y})");

            // 2. Создаём офис ЖКХ
            var utilityOfficeFactory = new UtilityOfficeFactory();
            var utilityOffice = utilityOfficeFactory.Create();
            var officePlacement = new Placement(new Position(25, 25), utilityOffice.Area);
            if (!_simulation.TryPlace(utilityOffice, officePlacement))
            {
                _messageService.ShowMessage("Не удалось разместить офис ЖКХ");
                return;
            }
            citizen.WorkPlace = (Building)utilityOffice;
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

        private void CreateTestJobScenario()
        {
            // 1. Создаём жителя
            var citizen = _citizenFactory.CreateCitizen(
                pos: new Position(15, 15),
                speed: 1.0f,
                profession: CitizenProfession.Chef
            );
            citizen.EducationLevel = EducationType.College;
            _simulation.AddCitizen(citizen);

            var citizen2 = _citizenFactory.CreateCitizen(
                pos: new Position(13, 16),
                speed: 1.0f,
                profession: CitizenProfession.Seller
            );
            citizen2.EducationLevel = EducationType.College;
            _simulation.AddCitizen(citizen2);
            //Debug.WriteLine($"Создан работник ЖКХ ID: {citizen.Id} на позиции ({citizen.Position.X}, {citizen.Position.Y})");

            // 2. Создаём кафе
            //var cafeFactory = new CafeFactory();
            //var cafe = cafeFactory.Create();
            //var cafePlacement = new Placement(new Position(25, 25), cafe.Area);
            //if (!_simulation.TryPlace(cafe, cafePlacement))
            //{
            //    _messageService.ShowMessage("Не удалось разместить кафе");
            //    return;
            //}
            //citizen.WorkPlace = (Building)cafe;
            //Debug.WriteLine($"Создан офис ЖКХ на позиции (25,25). Назначен как WorkPlace работнику {citizen.Id}");

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
            //_utilityService.BreakUtilityForTesting(residentialBuilding, UtilityType.Electricity, currentTick: 1);
            //var brokenUtilities = _utilityService.GetBrokenUtilities(residentialBuilding);
            //Debug.WriteLine($"Сломанные коммуналки в тестовом доме: {brokenUtilities.Count}");

            //// 7. Информация о тесте
            //_messageService.ShowMessage(
            //    "Тестовый сценарий создан!\n" +
            //    "1. Работник ЖКХ: (15,15)\n" +
            //    "2. Офис ЖКХ: (25,25)\n" +
            //    "3. Жилой дом: (35,35) - СЛОМАНО ЭЛЕКТРИЧЕСТВО\n\n" +
            //    "Работник должен побежать чинить сломанное ЖКХ."
            //);
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
            var now = DateTime.Now;
            var isDoubleClick = (_lastClickedTile == tile &&
                                (now - _lastTileClickTime).TotalMilliseconds < 500);

            _lastTileClickTime = now;
            _lastClickedTile = tile;

            // ПРОСТО: Двойной клик = устранить бедствие
            if (isDoubleClick && CurrentMode == MapInteractionMode.None)
            {
                if (tile.MapObject is Building building && building.Disasters.HasDisaster)
                {
                    // Просто убираем все бедствия
                    var activeDisasters = _disasterService.GetActiveDisasters(building);

                    foreach (var disaster in activeDisasters.Keys)
                    {
                        _disasterService.FixDisaster(building, disaster);
                    }

                    tile.UpdateBlinkingState();
                    _messageService.ShowMessage("Бедствие устранено!");
                    return;
                }
            }

            // Остальная логика одинарного клика остается как была
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

            // Убрали логику показа диалога бедствия для одинарного клика
            // (остается только показ диалога в старом методе, который не вызывается)

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

        // Убрали второй метод OnTileClicked, так как он был дублирован

        private void ShowDisasterDialog(Building building, TileVM tile)
        {
            var activeDisasters = _disasterService.GetActiveDisasters(building);

            if (!activeDisasters.Any())
            {
                _messageService.ShowMessage("Нет активных бедствий");
                return;
            }

            string message = "⚠️ АКТИВНЫЕ БЕДСТВИЯ:\n\n";

            foreach (var disaster in activeDisasters)
            {
                string disasterName = GetDisasterName(disaster.Key);
                string timeLeft = FormatTicks(disaster.Value);
                string effect = GetDisasterEffect(disaster.Key);

                message += $"{disasterName}\n";
                message += $"⏱️ Осталось: {timeLeft}\n";
                message += $"📝 {effect}\n\n";
            }

            // Просто показываем MessageBox
            System.Windows.MessageBox.Show(message, "Информация о бедствиях",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }

        private string GetDisasterName(DisasterType type)
        {
            return type switch
            {
                DisasterType.Fire => "🔥 ПОЖАР",
                DisasterType.Flood => "🌊 НАВОДНЕНИЕ",
                DisasterType.Blizzard => "❄️ МЕТЕЛЬ",
                _ => "БЕДСТВИЕ"
            };
        }

        private string GetDisasterEffect(DisasterType type)
        {
            return type switch
            {
                DisasterType.Fire => "Жители в панике, возможны жертвы",
                DisasterType.Flood => "Дороги затоплены, транспорт стоит",
                DisasterType.Blizzard => "Дороги занесены, видимость нулевая",
                _ => "Наносит ущерб зданию"
            };
        }

        private string FormatTicks(int ticks)
        {
            if (ticks <= 0) return "завершается...";

            return $"{ticks} тиков";
        }
    }
}