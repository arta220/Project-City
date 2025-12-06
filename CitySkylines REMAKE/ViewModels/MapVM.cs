using CitySimulatorWPF.Services;
using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Buildings;
using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Domain.Transports.Ground;
using Domain.Transports.States;
using Services;
using Services.CitizensSimulation;
using Services.TransportSimulation;
using Services.Utilities;
using System.Collections.ObjectModel;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel для основной карты города.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Управляет коллекциями тайлов (<see cref="TileVM"/>) и жителей (<see cref="CitizenVM"/>).
    /// - Обрабатывает взаимодействие пользователя с картой: клики, строительство, удаление объектов.
    /// - Инициирует симуляцию жителей и их движения.
    ///
    /// Контекст использования:
    /// - Связан с MainWindow для отображения карты и объектов.
    /// - Получает зависимости через DI (Simulation, RoadService, CitizenManager, MapTileService, MessageService, CitizenSimulationService).
    ///
    /// Взаимодействие с другими компонентами:
    /// - <see cref="Simulation"/> — размещение зданий и объектов на карте.
    /// - <see cref="IRoadConstructionService"/> — управление строительством дорог.
    /// - <see cref="ICitizenManagerService"/> и <see cref="CitizenSimulationService"/> — управление и обновление состояния жителей.
    /// - <see cref="IMapTileService"/> — инициализация и привязка тайлов к UI.
    ///
    /// Возможные расширения:
    /// - Добавление новых режимов взаимодействия с картой.
    /// - Поддержка разных типов объектов (коммерческие, культурные здания).
    /// - Расширение логики жителей (работа, учеба, досуг).
    /// </remarks>
    public partial class MapVM : ObservableObject
    {
        /// <summary>
        /// Выбранный объект для постройки.
        /// </summary>
        [ObservableProperty]
        private ObjectVM _selectedObject;

        /// <summary>
        /// Текущий режим взаимодействия с картой (строительство, удаление и т.д.).
        /// </summary>
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

        /// <summary>
        /// Коллекция тайлов карты для привязки к UI.
        /// </summary>
        public ObservableCollection<TileVM> Tiles => _mapTileService.Tiles;

        /// <summary>
        /// Коллекция жителей города для привязки к UI.
        /// </summary>
        public ObservableCollection<CitizenVM> Citizens => _citizenManager.Citizens;

        /// <summary>
        /// Коллекция личных машин для привязки к UI.
        /// </summary>
        public ObservableCollection<PersonalCarVM> Cars => _carManager.Cars;

        public int Width => _simulation.MapModel.Width;
        public int Height => _simulation.MapModel.Height;

        /// <summary>
        /// Создаёт ViewModel карты с переданными сервисами и симуляцией.
        /// </summary>
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
                    if (_roadService.IsBuilding)
                        _roadService.UpdatePreview(tile);
                    if (_pathService.IsBuilding) 
                        _pathService.UpdatePreview(tile);
                    return true;
                });

            // CreateTestScenarioCardboard(); Тестирование фабрики картона и фабрики упаковки
            CreateTestScenario();
            _utilityService = utilityService;
        }

        private void CreateTestPopulation(int count = 5)
        {
            var rand = new Random();

            for (int i = 0; i < count; i++)
            {
                var homeArea = new Area(1, 1);
                var homePos = new Position(rand.Next(0, Width), rand.Next(0, Height));
                var home = new ResidentialBuilding(1, 1, homeArea);
                _simulation.TryPlace(home, new Placement(homePos, home.Area));

                var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
                {
                    Age = rand.Next(18, 60),
                    Home = home,
                    Position = new Position(homePos.X, homePos.Y),
                    State = CitizenState.GoingHome
                };
                _simulation.AddCitizen(citizen);

                var workArea = new Area(1, 1);
                var workPos = new Position(rand.Next(0, Width), rand.Next(0, Height));
                var work = new Pharmacy(workArea);
                _simulation.TryPlace(work, new Placement(workPos, work.Area));
                citizen.WorkPlace = work;

                var car = new PersonalCar(new Area(1, 1), speed: 1.0f, owner: citizen)
                {
                    State = TransportState.IdleAtHome,
                    Position = new Position(homePos.X, homePos.Y)
                };
                citizen.PersonalCar = car;
                _simulation.AddTransport(car);

                car.Route.Clear();
                car.Route.Add(workPos);
            }
        }
        private void CreateTestScenarioCardboard()
        {
            // 1. Создаем дом для работника завода картона
            var homeFactory1 = new SmallHouseFactory();
            var workerHome1 = (ResidentialBuilding)homeFactory1.Create();
            var homePlacement1 = new Placement(new Position(5, 5), workerHome1.Area);

            if (!_simulation.TryPlace(workerHome1, homePlacement1))
            {
                _messageService.ShowMessage("Не удалось разместить дом работника завода картона");
                return;
            }

            // 2. Создаем дом для работника завода упаковки
            var homeFactory2 = new SmallHouseFactory();
            var workerHome2 = (ResidentialBuilding)homeFactory2.Create();
            var homePlacement2 = new Placement(new Position(10, 5), workerHome2.Area);

            if (!_simulation.TryPlace(workerHome2, homePlacement2))
            {
                _messageService.ShowMessage("Не удалось разместить дом работника завода упаковки");
                return;
            }

            // 3. Создаем завод по производству картона
            var cardboardFactory = new CardboardFactory();
            var cardboardBuilding = (IndustrialBuilding)cardboardFactory.Create();
            var cardboardPlacement = new Placement(new Position(15, 15), cardboardBuilding.Area);

            if (!_simulation.TryPlace(cardboardBuilding, cardboardPlacement))
            {
                _messageService.ShowMessage("Не удалось разместить завод картона");
                return;
            }

            // 4. Создаем завод по производству упаковки
            var packagingFactory = new PackagingFactory();
            var packagingBuilding = (IndustrialBuilding)packagingFactory.Create();
            var packagingPlacement = new Placement(new Position(25, 15), packagingBuilding.Area);

            if (!_simulation.TryPlace(packagingBuilding, packagingPlacement))
            {
                _messageService.ShowMessage("Не удалось разместить завод упаковки");
                return;
            }

            // 5. Создаем работника завода картона
            var worker1 = new Citizen(new Area(1, 1), speed: 1.0f)
            {
                Position = homePlacement1.Position, // Начинает у дома
                Home = workerHome1, // Устанавливаем дом
                WorkPlace = cardboardBuilding, // Рабочее место - завод картона
                State = CitizenState.Idle,
                Age = 30,
                Health = 100,
                Happiness = 50,
                Money = 1000
            };

            // 6. Создаем работника завода упаковки
            var worker2 = new Citizen(new Area(1, 1), speed: 1.0f)
            {
                Position = homePlacement2.Position, // Начинает у дома
                Home = workerHome2, // Устанавливаем дом
                WorkPlace = packagingBuilding, // Рабочее место - завод упаковки
                State = CitizenState.Idle,
                Age = 28,
                Health = 100,
                Happiness = 50,
                Money = 1000
            };

            // 7. Добавляем работников в симуляцию
            _simulation.AddCitizen(worker1);
            _simulation.AddCitizen(worker2);
            _messageService.ShowMessage("Тестовый сценарий создан!\n" +
                $"Дом работника картона на ({homePlacement1.Position.X},{homePlacement1.Position.Y})\n" +
                $"Дом работника упаковки на ({homePlacement2.Position.X},{homePlacement2.Position.Y})\n" +
                $"Завод картона на ({cardboardPlacement.Position.X},{cardboardPlacement.Position.Y})\n" +
                $"Завод упаковки на ({packagingPlacement.Position.X},{packagingPlacement.Position.Y})\n" +
                $"Работники на входах в дома");
        }

        private void CreateTestScenario()
        {
            var homeArea = new Area(1, 1);
            var homePos = new Position(5, 5);
            var home = new ResidentialBuilding(1, 1, homeArea);
            _simulation.TryPlace(home, new Placement(homePos, home.Area));

            var workArea = new Area(1, 1);
            var workPos = new Position(20, 20);
            var work = new Pharmacy(workArea);
            _simulation.TryPlace(work, new Placement(workPos, work.Area));

            var citizen = new Citizen(new Area(1, 1), speed: 1.0f)
            {
                Age = 25,
                Home = home,
                WorkPlace = work,
                Position = new Position(4, 5),
                State = CitizenState.GoingToWork
            };

            var car = new PersonalCar(new Area(1, 1), speed: 1.0f, owner: citizen)
            {
                State = TransportState.IdleAtHome,
                Position = new Position(2, 2)
            };
            citizen.PersonalCar = car;

            _simulation.AddCitizen(citizen);
            _simulation.AddTransport(car);
        }

        /// <summary>
        /// Обрабатывает начало строительства на тайле.
        /// </summary>
        private void OnTileConstructionStart(TileVM tile)
        {
            if (SelectedObject?.Factory is IRoadFactory)
            {
                _roadService.StartConstruction(tile);
            }
            else if (SelectedObject?.Factory is PedestrianPathFactory)
            {
                _pathService.StartConstruction(tile, PathType.Pedestrian);
            }
            else if (SelectedObject?.Factory is BicyclePathFactory)
            {
                _pathService.StartConstruction(tile, PathType.Bicycle);
            }

        }

        /// <summary>
        /// Обрабатывает клик по тайлу карты.
        /// </summary>
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

                CurrentMode = MapInteractionMode.None;
                return;
            }

            // Обработка клика для ремонта ЖКХ
            if (CurrentMode == MapInteractionMode.None && tile.MapObject is ResidentialBuilding residentialBuilding)
            {
                if (residentialBuilding.Utilities.HasBrokenUtilities)
                {
                    ShowRepairDialog(residentialBuilding, tile);
                }
            }

            if (CurrentMode == MapInteractionMode.Remove)
            {
                _simulation.TryRemove(tile.MapObject);
                return;
            }

            if (CurrentMode == MapInteractionMode.None)
            {
                // Возможные действия по клику, когда режим не выбран
            }
        }

        private void ShowRepairDialog(ResidentialBuilding building, TileVM tile)
        {
            // Получаем сломанные коммунальные услуги через сервис ЖКХ
            var brokenUtilities = _utilityService.GetBrokenUtilities(building);

            if (!brokenUtilities.Any())
            {
                _messageService.ShowMessage("Нет сломанных коммунальных услуг");
                return;
            }

            // Показываем список поломок
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

                // Обновляем визуальное состояние
                tile.UpdateBlinkingState();

                _messageService.ShowMessage($"{utilityToFix} отремонтирован!");
            }
        }
    }
}
