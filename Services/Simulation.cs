using Domain.Base;
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
            PlacementRepository placementRepository)
        {
            MapModel = mapModel ?? throw new ArgumentNullException(nameof(mapModel));
            _placementService = placementService ?? throw new ArgumentNullException(nameof(placementService));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _placementRepository = placementRepository ?? throw new ArgumentNullException(nameof(placementRepository));

            // Подписка на тики часов симуляции
            _clock.TickOccurred += OnTick;
            _clock.Start();
        }

        /// <summary>
        /// Обрабатывает событие тика от часов симуляции.
        /// </summary>
        private void OnTick(int tick)
        {
            TickOccurred?.Invoke(tick);
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
        public bool TryRemove(MapObject mapObject)
        {
            var placement = _placementRepository.GetPlacement(mapObject);

            if (_placementService.TryRemove(MapModel, placement))
                return true;

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
    }
}
