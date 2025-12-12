using CitySimulatorWPF.Services;
using CitySimulatorWPF.Views.dialogs;
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
using Services.Factories;
using Services.TransportSimulation;
using Services.Utilities;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using System.IO;
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

        private static void LogToFile(string message)
        {
            try
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CitySimulator", "debug.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
            }
            catch { }
        }

        partial void OnSelectedObjectChanged(ObjectVM value)
        {
            var msg = $"[MapVM] SelectedObject changed to: {value?.Factory?.GetType().Name ?? "null"}";
            Debug.WriteLine(msg);
            LogToFile(msg);
            // Если выбран объект, но режим не Build, устанавливаем режим Build
            if (value != null && CurrentMode != MapInteractionMode.Build)
            {
                var msg2 = $"[MapVM] Auto-setting CurrentMode to Build because SelectedObject is set";
                Debug.WriteLine(msg2);
                LogToFile(msg2);
                CurrentMode = MapInteractionMode.Build;
            }
        }

        partial void OnCurrentModeChanged(MapInteractionMode value)
        {
            var msg = $"[MapVM] CurrentMode changed to: {value}";
            Debug.WriteLine(msg);
            LogToFile(msg);
        }

        private readonly Simulation _simulation;
        private readonly IRoadConstructionService _roadService;
        private readonly ICitizenManagerService _citizenManager;
        private readonly ICarManagerService _carManager;
        private readonly IMapTileService _mapTileService;
        private readonly MessageService _messageService;
        private readonly IUtilityService _utilityService;
        private readonly IPathConstructionService _pathService;

        private bool _simulationStarted = false;
        private static bool _testScenarioCreated = false; // Защита от повторного вызова CreateTestScenario

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
            _citizenFactory = citizenFactory;
            _citizenManager.StartSimulation(citizenSimulation);
            _carManager.StartSimulation(transportSimulation);

            _mapTileService.InitializeTiles(
                _simulation.MapModel,
                onTileClicked: OnTileClicked,
                onTileDoubleClicked: OnTileDoubleClicked,
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

            // Защита от повторного вызова CreateTestScenario
            if (!_testScenarioCreated)
            {
                CreateTestScenario();
                _testScenarioCreated = true;
            }

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
            // 1. Создаём жителей (работников для заводов)
            var factoryWorker1 = _citizenFactory.CreateCitizen(
                pos: new Position(15, 15),
                speed: 1.0f,
                profession: CitizenProfession.FactoryWorker,
                state: CitizenState.Idle
            );

            var factoryWorker2 = _citizenFactory.CreateCitizen(
                pos: new Position(25, 15),
                speed: 1.0f,
                profession: CitizenProfession.FactoryWorker,
                state: CitizenState.Idle
            );

            var factoryWorker3 = _citizenFactory.CreateCitizen(
                pos: new Position(35, 15),
                speed: 1.0f,
                profession: CitizenProfession.FactoryWorker,
                state: CitizenState.Idle
            );

            _simulation.AddCitizen(factoryWorker1);
            _simulation.AddCitizen(factoryWorker2);
            _simulation.AddCitizen(factoryWorker3);

            Debug.WriteLine($"Созданы работники заводов: ID {factoryWorker1.Id}, {factoryWorker2.Id}, {factoryWorker3.Id}");

            // 2. Создаём ДОБЫВАЮЩИЙ ЗАВОД (ResourceExtractionFactory)
            var mineFactory = new ResourceExtractionFactory();
            var mineBuilding = mineFactory.Create() as Domain.Buildings.IndustrialBuilding;
            if (mineBuilding != null)
            {
                var minePlacement = new Placement(new Position(5, 5), mineBuilding.Area);
                if (_simulation.TryPlace(mineBuilding, minePlacement))
                {
                    Debug.WriteLine("Создан добывающий завод на позиции (5,5)");
                    // Назначаем рабочему место работы
                    factoryWorker1.WorkPlace = mineBuilding;
                    if (mineBuilding.Hire(factoryWorker1))
                    {
                        Debug.WriteLine($"Рабочий {factoryWorker1.Id} нанят на добывающий завод");
                    }
                }
            }

            // 3. Создаём ДЕРЕВООБРАБАТЫВАЮЩИЙ ЗАВОД (WoodProcessingFactory)
            var sawmillFactory = new WoodProcessingFactory();
            var sawmillBuilding = sawmillFactory.Create() as Domain.Buildings.IndustrialBuilding;
            if (sawmillBuilding != null)
            {
                var sawmillPlacement = new Placement(new Position(15, 5), sawmillBuilding.Area);
                if (_simulation.TryPlace(sawmillBuilding, sawmillPlacement))
                {
                    Debug.WriteLine("Создан деревообрабатывающий завод на позиции (15,5)");
                    // Назначаем рабочему место работы
                    factoryWorker2.WorkPlace = sawmillBuilding;
                    if (sawmillBuilding.Hire(factoryWorker2))
                    {
                        Debug.WriteLine($"Рабочий {factoryWorker2.Id} нанят на деревообрабатывающий завод");
                    }
                }
            }

            // 4. Создаём ПЕРЕРАБАТЫВАЮЩИЙ ЗАВОД (RecyclingFactory)
            var recyclingFactory = new RecyclingFactory();
            var recyclingBuilding = recyclingFactory.Create() as Domain.Buildings.IndustrialBuilding;
            if (recyclingBuilding != null)
            {
                var recyclingPlacement = new Placement(new Position(25, 5), recyclingBuilding.Area);
                if (_simulation.TryPlace(recyclingBuilding, recyclingPlacement))
                {
                    Debug.WriteLine("Создан перерабатывающий завод на позиции (25,5)");
                    // Назначаем рабочему место работы
                    factoryWorker3.WorkPlace = recyclingBuilding;
                    if (recyclingBuilding.Hire(factoryWorker3))
                    {
                        Debug.WriteLine($"Рабочий {factoryWorker3.Id} нанят на перерабатывающий завод");
                    }
                }
            }

            // 5. Создаём жилой дом с жителями (размещаем в другом месте, чтобы избежать конфликтов)
            var residentialFactory = new SmallHouseFactory();
            var residentialBuilding = (ResidentialBuilding)residentialFactory.Create();
            // Пробуем разместить дом в свободном месте
            var housePositions = new[] { new Position(35, 35), new Position(40, 40), new Position(45, 45), new Position(30, 30) };
            bool housePlaced = false;
            foreach (var pos in housePositions)
            {
                var housePlacement = new Placement(pos, residentialBuilding.Area);
                if (_simulation.TryPlace(residentialBuilding, housePlacement))
                {
                    Debug.WriteLine($"Создан жилой дом на позиции ({pos.X}, {pos.Y})");
                    housePlaced = true;
                    break;
                }
            }
            
            if (!housePlaced)
            {
                _messageService.ShowMessage("Не удалось разместить жилой дом");
                return;
            }

            // Создаём жителей в доме
            var residents = new List<Citizen>();
            for (int i = 0; i < 5; i++)
            {
                var resident = _citizenFactory.CreateCitizen(
                    pos: new Position(10 + i % 2, 10 + i / 2),
                    speed: 1.0f,
                    profession: CitizenProfession.FactoryWorker
                );
                _simulation.AddCitizen(resident);
                residents.Add(resident);
                Debug.WriteLine($"Создан житель ID: {resident.Id} на позиции ({resident.Position.X}, {resident.Position.Y})");
            }

            // Информация о сценарии
            _messageService.ShowMessage(
                "💪 ТЕСТ ПРОМЫШЛЕННОЙ ЦЕПОЧКИ\n\n" +
                "1. ДОБЫВАЮЩИЙ ЗАВОД (5,5) - ДВАЖДЫ КЛИКНИТЕ!\n" +
                "   • Производит: Железо, Дерево, Уголь\n" +
                "   • Рабочий: " + (mineBuilding?.GetWorkerCount() ?? 0) + "/" + (mineBuilding?.MaxOccupancy ?? 0) + "\n\n" +

                "2. ДЕРЕВООБРАБАТЫВАЮЩИЙ ЗАВОД (15,5) - ДВАЖДЫ КЛИКНИТЕ!\n" +
                "   • Производит: Пиломатериалы, Мебель, Бумагу, Ящики\n" +
                "   • Рабочий: " + (sawmillBuilding?.GetWorkerCount() ?? 0) + "/" + (sawmillBuilding?.MaxOccupancy ?? 0) + "\n\n" +

                "3. ПЕРЕРАБАТЫВАЮЩИЙ ЗАВОД (25,5) - ДВАFЖДЫ КЛИКНИТЕ!\n" +
                "   • Производит: Сталь, Пластик, Топливо, Бутылки\n" +
                "   • Рабочий: " + (recyclingBuilding?.GetWorkerCount() ?? 0) + "/" + (recyclingBuilding?.MaxOccupancy ?? 0) + "\n\n" +

                "4. ЖИЛОЙ ДОМ - 5 жителей\n\n" +

                "⚙️ КАК ПРОВЕРИТЬ:\n" +
                "• Дважды кликни по каждому заводу\n" +
                "• В диалоге найми еще рабочих (если есть вакансии)\n" +
                "• Смотри как меняются материалы и продукция\n" +
                "• Производство работает каждые 15 тиков\n" +
                "• Рабочие приходят на работу в рабочее время"
            );

            // Выводим в консоль информацию о цепочке производства
            Debug.WriteLine("\n=== ПРОМЫШЛЕННАЯ ЦЕПОЧКА ===");
            Debug.WriteLine("Добывающий завод → Дерево и Железо");
            Debug.WriteLine("Деревообрабатывающий завод → Пиломатериалы и Мебель");
            Debug.WriteLine("Перерабатывающий завод → Сталь и Пластик");
            Debug.WriteLine("=================================\n");
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

        private void OnTileDoubleClicked(TileVM tile)
        {
            if (CurrentMode == MapInteractionMode.None && tile.MapObject != null && tile.MapObject.GetType().Name.Contains("IndustrialBuilding"))
            {
                Debug.WriteLine($"Double click detected on IndustrialBuilding at ({tile.X}, {tile.Y})");
                ShowIndustrialBuildingDialog((Domain.Buildings.IndustrialBuilding)tile.MapObject, tile);
            }
            else
            {
                Debug.WriteLine($"Double click on tile ({tile.X}, {tile.Y}), but not IndustrialBuilding. Type: {tile.MapObject?.GetType().Name}, Mode: {CurrentMode}");
            }
        }

        private void OnTileClicked(TileVM tile)
        {
            var msg = $"[OnTileClicked] Mode={CurrentMode}, SelectedObject={SelectedObject?.Factory?.GetType().Name ?? "null"}, Tile=({tile.X}, {tile.Y})";
            Debug.WriteLine(msg);
            LogToFile(msg);

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
                var msg1 = $"[OnTileClicked] Attempting to place building: {SelectedObject.Factory.GetType().Name}";
                Debug.WriteLine(msg1);
                LogToFile(msg1);
                var obj = SelectedObject.Factory.Create();
                var placement = new Placement(new Position(tile.X, tile.Y), obj.Area);

                if (!_simulation.TryPlace(obj, placement))
                {
                    var msg2 = $"[OnTileClicked] Failed to place building at ({tile.X}, {tile.Y})";
                    Debug.WriteLine(msg2);
                    LogToFile(msg2);
                    _messageService.ShowMessage("Невозможно поставить объект");
                }
                else
                {
                    var msg3 = $"[OnTileClicked] Successfully placed building at ({tile.X}, {tile.Y})";
                    Debug.WriteLine(msg3);
                    LogToFile(msg3);
                    // Левый верхний тайл здания — якорный, на нём и показываем иконку
                    tile.IsMainObjectTile = true;
                }

                // Сбрасываем выбранный объект и режим после размещения
                SelectedObject = null;
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

        private void ShowIndustrialBuildingDialog(Domain.Buildings.IndustrialBuilding building, TileVM tile)
        {
            var dialog = new IndustrialBuildingInfoDialog(
                building,
                onHireWorker: (b) => HireWorkerForFactory(b, tile),
                onFireWorker: (b) => FireWorkerFromFactory(b)
            );
            dialog.ShowDialog();
        }

        private void HireWorkerForFactory(Domain.Buildings.IndustrialBuilding building, TileVM tile)
        {
            if (building.HasVacancy(CitizenProfession.FactoryWorker))
            {
                // Создаем нового рабочего
                var worker = new Citizen(new Area(1, 1), speed: 1.0f)
                {
                    Profession = CitizenProfession.FactoryWorker,
                    Age = 25 + new Random().Next(20), // 25-44 года
                    Position = new Position(tile.X + 1, tile.Y + 1),
                    Home = null, // У заводских рабочих может не быть дома
                    WorkPlace = null, // Важно: не устанавливаем WorkPlace заранее!
                    State = CitizenState.Idle,
                    Health = 100,
                    Happiness = 70,
                    Money = 500
                };

                // Пытаемся нанять
                if (building.Hire(worker))
                {
                    _simulation.AddCitizen(worker);
                    _messageService.ShowMessage($"Рабочий нанят на завод! Теперь рабочих: {building.GetWorkerCount()}/{building.MaxOccupancy}");
                }
                else
                {
                    _messageService.ShowMessage("Не удалось нанять рабочего");
                }
            }
            else
            {
                _messageService.ShowMessage("Нет свободных вакансий на заводе");
            }
        }

        private void FireWorkerFromFactory(Domain.Buildings.IndustrialBuilding building)
        {
            if (building.GetWorkerCount() > 0)
            {
                // Находим первого рабочего
                var workerToFire = building.CurrentWorkers.FirstOrDefault();
                if (workerToFire != null)
                {
                    // Увольняем
                    building.Fire(workerToFire);
                    _simulation.RemoveCitizen(workerToFire);
                    _messageService.ShowMessage($"Рабочий уволен с завода. Теперь рабочих: {building.GetWorkerCount()}/{building.MaxOccupancy}");
                }
            }
            else
            {
                _messageService.ShowMessage("На заводе нет рабочих для увольнения");
            }
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

        /// <summary>
        /// Очищает карту от всех объектов и тайлов.
        /// </summary>
        public void ClearMap()
        {
            _simulation.ClearMap();
            
            // Очищаем иконки зданий
            BuildingIcons.Clear();
            
            // Обновляем все тайлы
            foreach (var tile in Tiles)
            {
                tile.UpdateBlinkingState();
            }
            
            _messageService.ShowMessage("Карта очищена");
        }

        /// <summary>
        /// Обновляет иконки зданий после загрузки игры.
        /// </summary>
        public void RefreshBuildingIcons()
        {
            BuildingIcons.Clear();
            
            // Пересоздаем иконки для всех зданий на карте
            for (int x = 0; x < _simulation.MapModel.Width; x++)
            {
                for (int y = 0; y < _simulation.MapModel.Height; y++)
                {
                    var tile = _simulation.MapModel[x, y];
                    if (tile.MapObject != null)
                    {
                        var (placement, found) = _simulation.GetMapObjectPlacement(tile.MapObject);
                        if (found && placement != null)
                        {
                            // Проверяем, что это главный тайл здания (левый верхний угол)
                            if (placement.Value.Position.X == x && placement.Value.Position.Y == y)
                            {
                                const int tileSize = 20;
                                var iconVm = new BuildingIconVM(tile.MapObject, placement.Value, tileSize);
                                BuildingIcons.Add(iconVm);
                            }
                        }
                    }
                }
            }
        }
    }
}
