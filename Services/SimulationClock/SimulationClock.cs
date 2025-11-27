namespace Services.SimulationClock
{
    public class SimulationClock : ISimulationClock
    {
        private readonly System.Timers.Timer _timer;

        public int CurrentTick { get; private set; }

        public event Action<int> TickOccurred;

        public SimulationClock(int tickRateMs = 1000)
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
