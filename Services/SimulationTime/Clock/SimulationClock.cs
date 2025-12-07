namespace Services.Time.Clock
{
    /// <summary>
    /// Служба часов симуляции. Генерирует тики с заданным интервалом и уведомляет подписчиков.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - Служит источником времени для всех симуляционных сервисов, таких как
    ///   <see cref="CitizensSimulation.CitizenSimulationService"/> или <see cref="Simulation"/>.
    /// - Позволяет синхронизировать обновления состояния граждан, перемещение объектов и т.д.
    /// - Можно расширить, добавив паузу, изменение скорости симуляции или обработку нескольких параллельных потоков.
    /// </remarks>
    public class SimulationClock : ISimulationClock
    {
        private readonly System.Timers.Timer _timer;

        public int CurrentTick { get; private set; }

        public event Action<int> TickOccurred;
        public SimulationClock(int tickRateMs = 100)
        {
            _timer = new System.Timers.Timer(tickRateMs);
            _timer.Elapsed += (_, _) => Update();
        }
        public void Start() => _timer.Start();

        public void Stop() => _timer.Stop();
        public void Update()
        {
            CurrentTick++;
            TickOccurred?.Invoke(CurrentTick);
        }
    }
}
