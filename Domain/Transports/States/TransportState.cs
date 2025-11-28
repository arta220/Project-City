namespace Domain.Transports.States
{
    /// <summary>
    /// Простые состояния личного транспорта.
    /// </summary>
    public enum TransportState
    {
        /// <summary>
        /// Машина находится дома и стоит.
        /// </summary>
        IdleAtHome,

        /// <summary>
        /// Машина едет на работу.
        /// </summary>
        DrivingToWork,

        /// <summary>
        /// Машина припаркована у работы.
        /// </summary>
        ParkedAtWork,

        /// <summary>
        /// Машина едет домой.
        /// </summary>
        DrivingHome
    }
}
