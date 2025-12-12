using CitySimulatorWPF.Services;
using CitySimulatorWPF.Views.dialogs;
using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Buildings;
using Domain.Buildings.Logistics;
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

            // Подписка на событие размещения/удаления объектов
            _simulation.MapObjectPlaced += OnMapObjectPlaced;
            _simulation.MapObjectRemoved += OnMapObjectRemoved;

            // Создаем тестовый сценарий с химией и логистикой
            CreateIndustrialTestScenario();

            StartSimulationAfterUIReady();
        }

        private void CreateIndustrialTestScenario()
        {
            Debug.WriteLine("=== СОЗДАНИЕ ТЕСТОВОГО СЦЕНАРИЯ ХИМИЯ + ЛОГИСТИКА ===");

            // 1. СОЗДАЕМ РАБОЧИХ
            var chemist = _citizenFactory.CreateCitizen(
                pos: new Position(10, 10),
                speed: 1.0f,
                profession: CitizenProfession.Chemist
            );

            var logisticsManager = _citizenFactory.CreateCitizen(
                pos: new Position(15, 15),
                speed: 1.0f,
                profession: CitizenProfession.LogisticsManager
            );

            var factoryWorker1 = _citizenFactory.CreateCitizen(
                pos: new Position(20, 20),
                speed: 1.0f,
                profession: CitizenProfession.FactoryWorker
            );

            var factoryWorker2 = _citizenFactory.CreateCitizen(
                pos: new Position(25, 25),
                speed: 1.0f,
                profession: CitizenProfession.FactoryWorker
            );

            var truckDriver = _citizenFactory.CreateCitizen(
                pos: new Position(30, 30),
                speed: 1.0f,
                profession: CitizenProfession.TruckDriver
            );

            _simulation.AddCitizen(chemist);
            _simulation.AddCitizen(logisticsManager);
            _simulation.AddCitizen(factoryWorker1);
            _simulation.AddCitizen(factoryWorker2);
            _simulation.AddCitizen(truckDriver);

            // 2. СОЗДАЕМ ХИМИЧЕСКИЙ ЗАВОД (ConsumerChemicals специализация)
            var chemicalPlantFactory = new ChemicalPlantFactory();
            var chemicalPlant = chemicalPlantFactory.Create();
            var chemicalPlacement = new Placement(new Position(40, 40), chemicalPlant.Area);

            if (!_simulation.TryPlace(chemicalPlant, chemicalPlacement))
            {
                Debug.WriteLine("ОШИБКА: Не удалось разместить химический завод");
            }
            else
            {
                Debug.WriteLine($"✅ Химический завод создан в ({chemicalPlacement.Position.X},{chemicalPlacement.Position.Y})");

                // Нанять химика на завод
                if (chemicalPlant is Domain.Buildings.Industrial.ChemicalPlant plant)
                {
                    plant.Hire(chemist);
                    chemist.WorkPlace = plant;
                    Debug.WriteLine($"✅ Химик нанят на химический завод");

                    // Запустить производство
                    plant.RunOnce();
                    Debug.WriteLine($"✅ Производственный цикл запущен");

                    // Показать продукцию
                    foreach (var product in plant.ProductsBank)
                    {
                        if (product.Value > 0)
                        {
                            Debug.WriteLine($"   Продукция: {product.Key} = {product.Value}");
                        }
                    }
                }
            }

            // 3. СОЗДАЕМ ЛОГИСТИЧЕСКИЙ ЦЕНТР (ОБЯЗАТЕЛЬНО ДОБАВЛЕНО!)
            var logisticsCenterFactory = new LogisticsCenterFactory();
            var logisticsCenter = logisticsCenterFactory.Create();
            var logisticsPlacement = new Placement(new Position(50, 50), logisticsCenter.Area);

            if (!_simulation.TryPlace(logisticsCenter, logisticsPlacement))
            {
                Debug.WriteLine("ОШИБКА: Не удалось разместить логистический центр");
            }
            else
            {
                Debug.WriteLine($"✅ Логистический центр создан в ({logisticsPlacement.Position.X},{logisticsPlacement.Position.Y})");

                // Нанять логиста и водителя
                if (logisticsCenter is LogisticsCenter center)
                {
                    center.Hire(logisticsManager);
                    logisticsManager.WorkPlace = center;

                    center.Hire(truckDriver);
                    truckDriver.WorkPlace = center;

                    Debug.WriteLine($"✅ Логист и водитель наняты в логистический центр");

                    // Добавить тестовый транспорт
                    var deliveryVan = new Domain.Transports.Ground.DeliveryVan();
                    center.AddVehicle(deliveryVan);
                    Debug.WriteLine($"✅ Добавлен фургон для доставки");

                    // Принять тестовые товары на склад
                    center.ReceiveGoods(ProductType.Detergents, 100, new Position(40, 40));
                    center.ReceiveGoods(ProductType.Paints, 50, new Position(40, 40));
                    Debug.WriteLine($"✅ Товары приняты на склад логистического центра");
                }
            }

            // 4. СОЗДАЕМ ЖИЛОЙ ДОМ ДЛЯ РАБОЧИХ
            var residentialFactory = new SmallHouseFactory();
            var residentialBuilding = (ResidentialBuilding)residentialFactory.Create();
            var housePlacement = new Placement(new Position(60, 60), residentialBuilding.Area);

            if (_simulation.TryPlace(residentialBuilding, housePlacement))
            {
                Debug.WriteLine($"✅ Жилой дом создан в ({housePlacement.Position.X},{housePlacement.Position.Y})");

                // Поселить рабочих в дом
                factoryWorker1.Home = residentialBuilding;
                factoryWorker2.Home = residentialBuilding;
            }

            // 5. СОЗДАЕМ ЗАВОД УПАКОВКИ (для демонстрации связи)
            var packagingFactory = new PackagingFactory();
            var packagingBuilding = packagingFactory.Create() as Domain.Buildings.IndustrialBuilding;
            if (packagingBuilding != null)
            {
                var packagingPlacement = new Placement(new Position(30, 40), packagingBuilding.Area);
                if (_simulation.TryPlace(packagingBuilding, packagingPlacement))
                {
                    Debug.WriteLine($"✅ Завод упаковки создан в ({packagingPlacement.Position.X},{packagingPlacement.Position.Y})");

                    // Нанять рабочих
                    packagingBuilding.Hire(factoryWorker1);
                    factoryWorker1.WorkPlace = packagingBuilding;

                    packagingBuilding.Hire(factoryWorker2);
                    factoryWorker2.WorkPlace = packagingBuilding;

                    // Запустить производство
                    packagingBuilding.RunOnce();

                    // Отправить продукцию на логистический склад
                    if (packagingBuilding.ProductsBank.ContainsKey(ProductType.CardboardBox) &&
                        logisticsCenter is LogisticsCenter logistics)
                    {
                        int quantity = packagingBuilding.ProductsBank[ProductType.CardboardBox];
                        if (quantity > 0)
                        {
                            logistics.ReceiveGoods(ProductType.CardboardBox, quantity, packagingPlacement.Position);
                            Debug.WriteLine($"✅ {quantity} картонных коробок отправлено на логистический склад");
                        }
                    }
                }
            }

            // 6. СОЗДАЕМ ДОРОГИ ДЛЯ СВЯЗИ
            CreateTestRoads();

            // 7. ИНФОРМАЦИОННОЕ СООБЩЕНИЕ
            _messageService.ShowMessage(
                "ТЕСТОВЫЙ СЦЕНАРИЙ ХИМИЯ + ЛОГИСТИКА\n\n" +
                "✅ Созданы объекты:\n" +
                "1. Химический завод (40,40) - дважды кликните!\n" +
                "2. Логистический центр (50,50) - дважды кликните!\n" +
                "3. Завод упаковки (30,40) - дважды кликните!\n" +
                "4. Жилой дом (60,60)\n" +
                "5. 5 рабочих с разными профессиями\n\n" +
                "Химический завод производит:\n" +
                "   - Бытовую химию\n" +
                "   - Нефтехимическую продукцию\n\n" +
                "Логистический центр:\n" +
                "   - Управляет запасами\n" +
                "   - Организует доставки\n" +
                "   - Имеет транспорт\n\n" +
                "ДВАЖДЫ КЛИКНИТЕ по заводам для управления!"
            );

            Debug.WriteLine("=== ТЕСТОВЫЙ СЦЕНАРИЙ УСПЕШНО СОЗДАН ===");
        }

        private void CreateTestRoads()
        {
            // Создаем простую дорожную сеть для связи зданий
            var roadFactory = new RoadFactory();

            // Горизонтальная дорога от химического завода к логистическому центру
            for (int x = 40; x <= 50; x++)
            {
                var road = roadFactory.Create();
                var placement = new Placement(new Position(x, 45), road.Area);
                _simulation.TryPlace(road, placement);
            }

            // Вертикальная дорога от завода упаковки
            for (int y = 40; y <= 45; y++)
            {
                var road = roadFactory.Create();
                var placement = new Placement(new Position(35, y), road.Area);
                _simulation.TryPlace(road, placement);
            }

            Debug.WriteLine("✅ Созданы тестовые дороги");
        }

        private void OnMapObjectPlaced(MapObject mapObject)
        {
            var (placement, found) = _simulation.GetMapObjectPlacement(mapObject);
            if (!found || placement is null)
                return;

            const int tileSize = 20;

            var iconVm = new BuildingIconVM(mapObject, (Placement)placement, tileSize);
            BuildingIcons.Add(iconVm);

            // Логирование для отладки
            Debug.WriteLine($"📌 Размещен объект: {mapObject.GetType().Name} в ({placement.Value.Position.X},{placement.Value.Position.Y})");
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
            if (CurrentMode == MapInteractionMode.None && tile.MapObject != null)
            {
                var mapObject = tile.MapObject;
                var type = mapObject.GetType();

                Debug.WriteLine($"🖱️ Двойной клик на объекте: {type.Name} в ({tile.X},{tile.Y})");
                Debug.WriteLine($"   Полное имя типа: {type.FullName}");
                Debug.WriteLine($"   Is ChemicalPlant: {mapObject is Domain.Buildings.Industrial.ChemicalPlant}");
                Debug.WriteLine($"   Is LogisticsCenter: {mapObject is LogisticsCenter}");
                Debug.WriteLine($"   Is IndustrialBuilding: {mapObject is Domain.Buildings.IndustrialBuilding}");
                Debug.WriteLine($"   Base type: {type.BaseType?.Name}");

                // ВАЖНО: Проверяем сначала производные классы, потом базовые!
                // Порядок проверки имеет значение!

                if (mapObject is Domain.Buildings.Industrial.ChemicalPlant chemicalPlant)
                {
                    Debug.WriteLine("🔬 Открываем диалог химического завода");
                    ShowChemicalPlantDialog(chemicalPlant, tile);
                }
                else if (mapObject is LogisticsCenter logisticsCenter) // ← ДО IndustrialBuilding!
                {
                    Debug.WriteLine("🚚 Открываем диалог логистического центра");
                    ShowLogisticsCenterDialog(logisticsCenter, tile);
                }
                else if (mapObject is Domain.Buildings.IndustrialBuilding industrialBuilding)
                {
                    Debug.WriteLine("🏭 Открываем стандартный диалог промышленного здания");
                    ShowIndustrialBuildingDialog(industrialBuilding, tile);
                }
                else
                {
                    Debug.WriteLine($"ℹ️ Объект типа {mapObject.GetType().Name} не поддерживает диалоговое окно");
                }
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

        // ===== ДИАЛОГОВЫЕ ОКНА ДЛЯ НОВЫХ ЗДАНИЙ =====

        private void ShowChemicalPlantDialog(Domain.Buildings.Industrial.ChemicalPlant plant, TileVM tile)
        {
            var dialog = new ChemicalPlantDialog(
                plant,
                onRunProduction: (p) => RunChemicalPlantProduction(p),
                onUpgradeTechnology: (p) => UpgradeChemicalPlant(p),
                onHireWorker: (p) => HireWorkerForChemicalPlant(p, tile),
                onFireWorker: (p) => FireWorkerFromChemicalPlant(p)
            );
            dialog.ShowDialog();
        }

        private void ShowLogisticsCenterDialog(LogisticsCenter center, TileVM tile)
        {
            var dialog = new LogisticsCenterDialog(
                center,
                onProcessLogistics: (c) => ProcessLogistics(c),
                onPrepareShipment: (c) => PrepareShipmentFromLogistics(c),
                onHireWorker: (c) => HireWorkerForLogistics(c, tile),
                onFireWorker: (c) => FireWorkerFromLogistics(c)
            );
            dialog.ShowDialog();
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

        // ===== МЕТОДЫ ДЛЯ ХИМИЧЕСКОГО ЗАВОДА =====

        private void RunChemicalPlantProduction(Domain.Buildings.Industrial.ChemicalPlant plant)
        {
            plant.RunOnce();

            // Показать результаты производства
            string message = "Производственный цикл выполнен!\n\nПродукция:\n";

            bool hasProduction = false;
            foreach (var product in plant.ProductsBank)
            {
                if (product.Value > 0)
                {
                    message += $"- {product.Key}: {product.Value} ед.\n";
                    hasProduction = true;
                }
            }

            if (!hasProduction)
            {
                message += "Нет произведенной продукции (возможно, нет сырья)";
            }
            else
            {
                message += $"\nЗагрязнение: {plant.PollutionLevel}%\n";
                message += $"Безопасность: {plant.SafetyLevel}%";
            }

            _messageService.ShowMessage(message);
        }

        private void UpgradeChemicalPlant(Domain.Buildings.Industrial.ChemicalPlant plant)
        {
            plant.UpgradeTechnology();
            _messageService.ShowMessage($"Технологический уровень повышен до {plant.TechnologyLevel}\nЭффективность производства увеличена!");
        }

        private void HireWorkerForChemicalPlant(Domain.Buildings.Industrial.ChemicalPlant plant, TileVM tile)
        {
            if (plant.HasVacancy(CitizenProfession.Chemist) || plant.HasVacancy(CitizenProfession.FactoryWorker))
            {
                // Определяем профессию для найма
                var profession = plant.HasVacancy(CitizenProfession.Chemist)
                    ? CitizenProfession.Chemist
                    : CitizenProfession.FactoryWorker;

                var worker = new Citizen(new Area(1, 1), speed: 1.0f)
                {
                    Profession = profession,
                    Age = 25 + new Random().Next(20),
                    Position = new Position(tile.X + 1, tile.Y + 1),
                    Home = null,
                    WorkPlace = null,
                    State = CitizenState.Idle,
                    Health = 100,
                    Happiness = 70,
                    Money = 500
                };

                if (plant.Hire(worker))
                {
                    _simulation.AddCitizen(worker);
                    _messageService.ShowMessage($"{profession} нанят на химический завод!\n" +
                                               $"Теперь рабочих: {plant.GetWorkerCount()}/{plant.MaxOccupancy}");
                }
            }
            else
            {
                _messageService.ShowMessage("Нет свободных вакансий на химическом заводе");
            }
        }

        private void FireWorkerFromChemicalPlant(Domain.Buildings.Industrial.ChemicalPlant plant)
        {
            if (plant.GetWorkerCount() > 0)
            {
                var workerToFire = plant.CurrentWorkers.FirstOrDefault();
                if (workerToFire != null)
                {
                    plant.Fire(workerToFire);
                    _simulation.RemoveCitizen(workerToFire);
                    _messageService.ShowMessage($"Рабочий уволен с химического завода\n" +
                                               $"Теперь рабочих: {plant.GetWorkerCount()}/{plant.MaxOccupancy}");
                }
            }
            else
            {
                _messageService.ShowMessage("На химическом заводе нет рабочих для увольнения");
            }
        }

        // ===== МЕТОДЫ ДЛЯ ЛОГИСТИЧЕСКОГО ЦЕНТРА =====

        private void ProcessLogistics(LogisticsCenter center)
        {
            center.ProcessLogistics();

            var stats = center.GetStatistics();
            string message = "Логистика обработана!\n\n";
            message += $"Товаров на складе: {stats.TotalStock} ед.\n";
            message += $"Свободно места: {stats.AvailableCapacity} ед.\n";
            message += $"Ожидающих заказов: {stats.PendingOrders}\n";
            message += $"Активных доставок: {stats.ActiveDeliveries}\n";
            message += $"Эффективность: {stats.Efficiency}%\n";
            message += $"Транспорт: {stats.VehicleCount} ед.";

            _messageService.ShowMessage(message);
        }

        private void PrepareShipmentFromLogistics(LogisticsCenter center)
        {
            // Создаем тестовый заказ на доставку
            var destination = new Position(60, 60); // Жилой дом
            var destinationBuilding = new SmallHouseFactory().Create() as Building;

            if (center.WarehouseStock.ContainsKey(ProductType.Detergents) && center.WarehouseStock[ProductType.Detergents] > 0)
            {
                var order = center.CreateDeliveryOrder(
                    ProductType.Detergents,
                    10,
                    destination,
                    destinationBuilding
                );

                if (order != null)
                {
                    _messageService.ShowMessage($"Заказ на доставку создан!\n" +
                                               $"Товар: {order.Product}\n" +
                                               $"Количество: {order.Quantity} ед.\n" +
                                               $"Назначение: ({destination.X},{destination.Y})");
                }
                else
                {
                    _messageService.ShowMessage("Не удалось создать заказ на доставку");
                }
            }
            else
            {
                _messageService.ShowMessage("Нет товаров для отгрузки");
            }
        }

        private void HireWorkerForLogistics(LogisticsCenter center, TileVM tile)
        {
            if (center.HasVacancy(CitizenProfession.LogisticsManager) ||
                center.HasVacancy(CitizenProfession.TruckDriver) ||
                center.HasVacancy(CitizenProfession.WarehouseWorker))
            {
                // Определяем профессию для найма
                CitizenProfession profession;
                if (center.HasVacancy(CitizenProfession.LogisticsManager))
                    profession = CitizenProfession.LogisticsManager;
                else if (center.HasVacancy(CitizenProfession.TruckDriver))
                    profession = CitizenProfession.TruckDriver;
                else
                    profession = CitizenProfession.WarehouseWorker;

                var worker = new Citizen(new Area(1, 1), speed: 1.0f)
                {
                    Profession = profession,
                    Age = 25 + new Random().Next(20),
                    Position = new Position(tile.X + 1, tile.Y + 1),
                    Home = null,
                    WorkPlace = null,
                    State = CitizenState.Idle,
                    Health = 100,
                    Happiness = 70,
                    Money = 500
                };

                if (center.Hire(worker))
                {
                    _simulation.AddCitizen(worker);
                    _messageService.ShowMessage($"{profession} нанят в логистический центр!\n" +
                                               $"Теперь сотрудников: {center.GetWorkerCount()}/{center.MaxOccupancy}");
                }
            }
            else
            {
                _messageService.ShowMessage("Нет свободных вакансий в логистическом центре");
            }
        }

        private void FireWorkerFromLogistics(LogisticsCenter center)
        {
            if (center.GetWorkerCount() > 0)
            {
                var workerToFire = center.CurrentWorkers.FirstOrDefault();
                if (workerToFire != null)
                {
                    center.Fire(workerToFire);
                    _simulation.RemoveCitizen(workerToFire);
                    _messageService.ShowMessage($"Сотрудник уволен из логистического центра\n" +
                                               $"Теперь сотрудников: {center.GetWorkerCount()}/{center.MaxOccupancy}");
                }
            }
            else
            {
                _messageService.ShowMessage("В логистическом центре нет сотрудников для увольнения");
            }
        }

        // ===== СТАНДАРТНЫЕ МЕТОДЫ ДЛЯ ПРОМЫШЛЕННЫХ ЗДАНИЙ =====

        private void HireWorkerForFactory(Domain.Buildings.IndustrialBuilding building, TileVM tile)
        {
            if (building.HasVacancy(CitizenProfession.FactoryWorker))
            {
                var worker = new Citizen(new Area(1, 1), speed: 1.0f)
                {
                    Profession = CitizenProfession.FactoryWorker,
                    Age = 25 + new Random().Next(20),
                    Position = new Position(tile.X + 1, tile.Y + 1),
                    Home = null,
                    WorkPlace = null,
                    State = CitizenState.Idle,
                    Health = 100,
                    Happiness = 70,
                    Money = 500
                };

                if (building.Hire(worker))
                {
                    _simulation.AddCitizen(worker);
                    _messageService.ShowMessage($"Рабочий нанят на завод! Теперь рабочих: {building.GetWorkerCount()}/{building.MaxOccupancy}");
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
                var workerToFire = building.CurrentWorkers.FirstOrDefault();
                if (workerToFire != null)
                {
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
    }
}