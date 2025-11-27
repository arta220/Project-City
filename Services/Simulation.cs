using Domain.Base;
using Domain.Buildings;
using Domain.Enums;
using Domain.Map;
using Services.Interfaces;
using Services.PlaceBuilding;
using Services.SimulationClock;
using System;
using System.Collections.ObjectModel;

namespace Services
{
    /// <summary>
    /// Центральный сервис симуляции города. Отвечает за:
    /// - хранение карты (<see cref="MapModel"/>),
    /// - размещение объектов на карте,
    /// - уведомление о тиках симуляции и изменениях объектов.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Источник событий времени для всех сервисов (например, граждан, строительства).
    /// - Управление объектами на карте через <see cref="IMapObjectPlacementService"/> и <see cref="PlacementRepository"/>.
    /// - Можно расширить: добавлять удаление объектов, взаимодействие граждан с объектами, динамическое изменение карты.
    /// </remarks>
    public class Simulation
    {
        private readonly IMapObjectPlacementService _placementService;
        private readonly ISimulationClock _clock;
        private readonly PlacementRepository _placementRepository;
        private readonly IUtilityService _utilityService;

        /// <summary>
        /// Событие, вызываемое на каждом тике симуляции.
        /// </summary>
        public event Action<int> TickOccurred;

        /// <summary>
        /// Событие, вызываемое при успешном размещении объекта на карте.
        /// </summary>
        public event Action<MapObject> MapObjectPlaced;

        /// <summary>
        /// Событие, вызываемое при удалении объекта с карты.
        /// </summary>
        public event Action<MapObject> MapObjectRemoved;

        /// <summary>
        /// Модель карты города.
        /// </summary>
        public MapModel MapModel { get; private set; }

        /// <summary>
        /// Коллекция всех размещённых объектов на карте.
        /// </summary>
        public ObservableCollection<MapObject> MapObjects { get; } = new ObservableCollection<MapObject>();

        /// <summary>
        /// Создаёт экземпляр симуляции.
        /// </summary>
        /// <param name="mapModel">Модель карты.</param>
        /// <param name="placementService">Сервис размещения объектов на карте.</param>
        /// <param name="clock">Часы симуляции.</param>
        /// <param name="placementRepository">Репозиторий размещений объектов.</param>
        public Simulation(
            MapModel mapModel,
            IMapObjectPlacementService placementService,
            ISimulationClock clock,
            PlacementRepository placementRepository,
            IUtilityService utilityService)
        {
            MapModel = mapModel ?? throw new ArgumentNullException(nameof(mapModel));
            _placementService = placementService ?? throw new ArgumentNullException(nameof(placementService));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _placementRepository = placementRepository ?? throw new ArgumentNullException(nameof(placementRepository));
            _utilityService = utilityService;

            // Подписка на тики часов симуляции
            _clock.TickOccurred += OnTick;
            _clock.TickOccurred += OnUtilityTick;
            _clock.Start();
        }

        /// <summary>
        /// Обрабатывает событие тика от часов симуляции.
        /// </summary>
        private void OnTick(int tick)
        {
            TickOccurred?.Invoke(tick);
        }

        private void OnUtilityTick(int tick)
        {
            // Получаем только жилые здания для симуляции ЖКХ
            var residentialBuildings = MapObjects.OfType<ResidentialBuilding>().ToList();
            _utilityService.SimulateUtilitiesBreakdown(tick, residentialBuildings.Cast<Building>().ToList());
        }

        public Dictionary<UtilityType, int> GetBrokenUtilities(Building building)
        {
            return _utilityService.GetBrokenUtilities(building);
        }

        public void FixBuildingUtility(Building building, UtilityType utilityType)
        {
            _utilityService.FixUtility(building, utilityType);
        }

        /// <summary>
        /// Пытается разместить объект на карте.
        /// </summary>
        /// <param name="mapObject">Объект для размещения.</param>
        /// <param name="placement">Позиция и размеры объекта.</param>
        /// <returns>True, если размещение прошло успешно.</returns>
        public bool TryPlace(MapObject mapObject, Placement placement)
        {
            if (_placementService.TryPlace(MapModel, mapObject, placement))
            {
                _placementRepository.Register(mapObject, placement);
                MapObjects.Add(mapObject);
                MapObjectPlaced?.Invoke(mapObject);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Удаляет объект с карты (пока не реализован полностью).
        /// </summary>
        public bool RemoveMapObject(MapObject mapObject)
        {
            // TODO: реализовать корректное удаление с уведомлением
            return false;
        }

        /// <summary>
        /// Получает размещение объекта на карте.
        /// </summary>
        public Placement GetMapObjectPlacement(MapObject mapObject)
        {
            return _placementRepository.GetPlacement(mapObject);
        }

        /// <summary>
        /// Проверяет, можно ли разместить объект на карте в указанной позиции.
        /// </summary>
        public bool CanPlace(MapObject mapObject, Placement placement)
        {
            return _placementService.CanPlace(MapModel, mapObject, placement);
        }

        // Smirnov *
        public void RemoveBuilding(MapObject mapObject)
        {
            // Получаем размещение объекта
            var placement = _placementRepository.GetPlacement(mapObject);

            // Убираем объект со всех тайлов в области размещения
            for (int x = placement.Position.X; x < placement.Position.X + placement.Area.Width; x++)
            {
                for (int y = placement.Position.Y; y < placement.Position.Y + placement.Area.Height; y++)
                {
                    if (x < MapModel.Width && y < MapModel.Height &&
                        MapModel[x, y].MapObject == mapObject)
                    {
                        MapModel[x, y].MapObject = null;
                    }
                }
            }

            // Удаляем из репозитория
            _placementRepository.Remove(mapObject);
        }
    }
}
