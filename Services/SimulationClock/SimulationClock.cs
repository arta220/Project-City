using System;
using System.Timers;

namespace Services.SimulationClock
{
    /// <summary>
    /// Служба часов симуляции. Генерирует тики с заданным интервалом и уведомляет подписчиков.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Служит источником времени для всех симуляционных сервисов, таких как
    ///   <see cref="Services.CitizensSimulation.CitizenSimulationService"/> или <see cref="Services.Simulation"/>.
    /// - Позволяет синхронизировать обновления состояния граждан, перемещение объектов и т.д.
    /// - Можно расширить, добавив паузу, изменение скорости симуляции или обработку нескольких параллельных потоков.
    /// </remarks>
    public class SimulationClock : ISimulationClock
    {
        private readonly System.Timers.Timer _timer;

        /// <summary>
        /// Текущий тик симуляции.
        /// </summary>
        public int CurrentTick { get; private set; }

        /// <summary>
        /// Событие, вызываемое при каждом тике симуляции.
        /// </summary>
        public event Action<int> TickOccurred;

        /// <summary>
        /// Создаёт экземпляр часов симуляции.
        /// </summary>
        /// <param name="tickRateMs">Интервал между тиками в миллисекундах. По умолчанию 1000 мс.</param>
        public SimulationClock(int tickRateMs = 1000)
        {
            _timer = new System.Timers.Timer(tickRateMs);
            _timer.Elapsed += (_, _) => Update();
        }

        /// <summary>
        /// Запускает симуляцию (начинает генерировать тики).
        /// </summary>
        public void Start() => _timer.Start();

        /// <summary>
        /// Останавливает симуляцию.
        /// </summary>
        public void Stop() => _timer.Stop();

        /// <summary>
        /// Обновляет состояние часов, увеличивает текущий тик и уведомляет подписчиков.
        /// </summary>
        public void Update()
        {
            CurrentTick++;
            TickOccurred?.Invoke(CurrentTick);
        }
    }
}
