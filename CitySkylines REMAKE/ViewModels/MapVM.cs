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
            if (CurrentMode == MapInteractionMode.Build && SelectedObject?.Model is Road)
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
    }
}
