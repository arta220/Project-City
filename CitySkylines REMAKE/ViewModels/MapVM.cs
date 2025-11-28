using CitySimulatorWPF.Services;
using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Base;
using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Factories;
using Domain.Map;
using Services;
using Services.CitizensSimulation;
using System.Collections.ObjectModel;
using System.Windows;
using Services.Interfaces;

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
        private readonly IMapTileService _mapTileService;
        private readonly MessageService _messageService;
        private readonly CitizenSimulationService _citizenSimulation;
        private readonly IUtilityService _utilityService;

        /// <summary>
        /// Коллекция тайлов карты для привязки к UI.
        /// </summary>
        public ObservableCollection<TileVM> Tiles => _mapTileService.Tiles;

        /// <summary>
        /// Коллекция жителей города для привязки к UI.
        /// </summary>
        public ObservableCollection<CitizenVM> Citizens => _citizenManager.Citizens;

        public int Width => _simulation.MapModel.Width;
        public int Height => _simulation.MapModel.Height;

        /// <summary>
        /// Создаёт ViewModel карты с переданными сервисами и симуляцией.
        /// </summary>
        public MapVM(Simulation simulation,
                     IRoadConstructionService roadService,
                     ICitizenManagerService citizenManager,
                     IMapTileService mapTileService,
                     MessageService messageService,
                     CitizenSimulationService citizenSimulation,
                     IUtilityService utilityService)
        {
            _simulation = simulation;
            _roadService = roadService;
            _citizenManager = citizenManager;
            _mapTileService = mapTileService;
            _messageService = messageService;
            _citizenSimulation = citizenSimulation;
            _utilityService = utilityService;

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
            _utilityService = utilityService;
        }

        /// <summary>
        /// Создаёт начальное жилище и жителя для тестирования симуляции.
        /// </summary>
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

        /// <summary>
        /// Обрабатывает начало строительства на тайле.
        /// </summary>
        private void OnTileConstructionStart(TileVM tile)
        {
            if (SelectedObject?.Factory is IRoadFactory)
            {
                _roadService.StartConstruction(tile);
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


            if (CurrentMode == MapInteractionMode.Remove)
            {
                if (!_simulation.TryRemove(tile.MapObject))
                {
                    _messageService.ShowMessage("Невозможно удалить объект");
                }
                return;
            }

            if (CurrentMode == MapInteractionMode.None)
            {
                // Возможные действия по клику, когда режим не выбран
            }
        }

        /// <summary>
        /// Очищает ресурсы и останавливает симуляцию жителей.
        /// </summary>
        public void Cleanup()
        {
            _citizenManager.StopSimulation();
            _citizenSimulation.Stop();
        }

        private void ShowRepairDialog(Domain.Base.Building building, TileVM tile)
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
