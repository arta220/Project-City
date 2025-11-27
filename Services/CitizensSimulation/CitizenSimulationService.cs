using Domain.Citizens;
using Services.SimulationClock;
using System;
using System.Collections.ObjectModel;

namespace Services.CitizensSimulation
{
    /// <summary>
    /// Сервис симуляции граждан. Управляет коллекцией граждан и обновляет их состояние на каждом тике симуляции.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Используется в <see cref="MapVM"/> через <see cref="CitizenManagerService"/> для визуального отображения граждан.
    /// - Получает тик симуляции через <see cref="ISimulationClock"/>.
    /// - Делегирует обновление поведения каждого гражданина контроллеру <see cref="CitizenController"/>.
    /// - Поддерживает события <see cref="CitizenAdded"/>, <see cref="CitizenRemoved"/> и <see cref="CitizenUpdated"/>,
    ///   чтобы VM могла обновлять визуальные объекты на карте.
    /// 
    /// Возможные расширения:
    /// - Добавить фильтры или группировки граждан по состоянию, работе или дому.
    /// - Реализовать паузы и пошаговое управление симуляцией.
    /// - Добавить события для других действий (например, вход в здание, покупка ресурсов и т.п.).
    /// </remarks>
    public class CitizenSimulationService
    {
        private readonly CitizenController _controller;
        private readonly ISimulationClock _clock;

        /// <summary>
        /// Коллекция всех граждан, участвующих в симуляции.
        /// </summary>
        public ObservableCollection<Citizen> Citizens { get; } = new();

        /// <summary>
        /// Событие, вызываемое при добавлении нового гражданина.
        /// </summary>
        public event Action<Citizen> CitizenAdded;

        /// <summary>
        /// Событие, вызываемое при удалении гражданина из симуляции.
        /// </summary>
        public event Action<Citizen> CitizenRemoved;

        /// <summary>
        /// Событие, вызываемое при обновлении состояния гражданина на каждом тике.
        /// </summary>
        public event Action<Citizen> CitizenUpdated;

        /// <summary>
        /// Создаёт сервис симуляции граждан с указанным контроллером поведения и источником тиков.
        /// </summary>
        /// <param name="controller">Контроллер для обновления состояния граждан.</param>
        /// <param name="clock">Источник тиков симуляции.</param>
        public CitizenSimulationService(
            CitizenController controller,
            ISimulationClock clock)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        /// <summary>
        /// Запускает симуляцию: подписка на события тиков.
        /// </summary>
        public void Start()
        {
            _clock.TickOccurred += UpdateAll;
        }

        /// <summary>
        /// Останавливает симуляцию: отписка от событий тиков.
        /// </summary>
        public void Stop()
        {
            _clock.TickOccurred -= UpdateAll;
        }

        /// <summary>
        /// Добавляет нового гражданина в симуляцию.
        /// </summary>
        /// <param name="citizen">Гражданин для добавления.</param>
        public void AddCitizen(Citizen citizen)
        {
            if (citizen == null) throw new ArgumentNullException(nameof(citizen));

            Citizens.Add(citizen);
            CitizenAdded?.Invoke(citizen);
        }

        /// <summary>
        /// Удаляет гражданина из симуляции.
        /// </summary>
        /// <param name="citizen">Гражданин для удаления.</param>
        public void RemoveCitizen(Citizen citizen)
        {
            if (Citizens.Remove(citizen))
            {
                CitizenRemoved?.Invoke(citizen);
            }
        }

        /// <summary>
        /// Обновляет всех граждан на текущем тике симуляции.
        /// Вызывается автоматически при событии тика из <see cref="ISimulationClock"/>.
        /// </summary>
        /// <param name="tick">Номер текущего тика.</param>
        public void UpdateAll(int tick)
        {
            foreach (var citizen in Citizens)
            {
                _controller.UpdateCitizen(citizen, tick);
                CitizenUpdated?.Invoke(citizen);
            }
        }
    }
}
