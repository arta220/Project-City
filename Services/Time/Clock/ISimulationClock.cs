namespace Services.Time.Clock
{
    public interface ISimulationClock
    {
        event Action<int> TickOccurred;
        int CurrentTick { get; }
        void Start();
        void Stop();
        void Update();
    }
}
