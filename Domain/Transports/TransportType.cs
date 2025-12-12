namespace Domain.Transports
{
    public enum TransportType
    {
        Bus, Taxi, Car,
        // Новые для логистики
        DeliveryVan,     // Маленький фургон для доставки
        CargoTruck,      // Грузовик средней грузоподъемности
        SemiTrailerTruck // Фура с полуприцепом
    }
}
