using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Buildings;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;
using Domain.Transports.Ground;

namespace Domain.Buildings.Logistics
{
    /// <summary>
    /// Логистический центр - управляет поставками, хранением и распределением товаров
    /// </summary>
    public class LogisticsCenter : IndustrialBuilding
    {
        /// <summary>
        /// Складские запасы (продукт -> количество)
        /// </summary>
        public Dictionary<ProductType, int> WarehouseStock { get; private set; } = new();

        /// <summary>
        /// Текущие заказы на поставку
        /// </summary>
        public Dictionary<ProductType, List<DeliveryOrder>> ActiveOrders { get; private set; } = new();

        /// <summary>
        /// Доступный транспортный парк
        /// </summary>
        public List<Transport> AvailableVehicles { get; private set; } = new();

        /// <summary>
        /// История доставок
        /// </summary>
        public List<DeliveryRecord> DeliveryHistory { get; private set; } = new();

        /// <summary>
        /// Вместимость склада (максимальное количество единиц товара)
        /// </summary>
        public int WarehouseCapacity { get; private set; }

        /// <summary>
        /// Текущая загруженность склада
        /// </summary>
        public int CurrentWarehouseLoad
        {
            get { return WarehouseStock.Values.Sum(); }
        }

        /// <summary>
        /// Эффективность логистики (0-100%)
        /// </summary>
        public int LogisticsEfficiency { get; private set; }

        public LogisticsCenter(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area, IndustrialBuildingType.Logistics)
        {
            WarehouseCapacity = 5000; // 5000 единиц товара
            LogisticsEfficiency = 75;

            Initialize();
        }

        private void Initialize()
        {
            // Настройка вакансий
            Vacancies[CitizenProfession.LogisticsManager] = 2;
            Vacancies[CitizenProfession.WarehouseWorker] = 15;
            Vacancies[CitizenProfession.TruckDriver] = 8;
            Vacancies[CitizenProfession.Dispatcher] = 1;

            // Настройка максимального возраста
            MaxAges[CitizenProfession.LogisticsManager] = 65;
            MaxAges[CitizenProfession.WarehouseWorker] = 55;
            MaxAges[CitizenProfession.TruckDriver] = 60;
            MaxAges[CitizenProfession.Dispatcher] = 62;

            // Инициализация склада с базовыми товарами
            InitializeBasicStock();
        }

        private void InitializeBasicStock()
        {
            // Начальные запасы для тестирования
            WarehouseStock[ProductType.FoodContainer] = 100;
            WarehouseStock[ProductType.CardboardBox] = 200;
            WarehouseStock[ProductType.PlasticBottle] = 150;
        }

        /// <summary>
        /// Принять товар на склад
        /// </summary>
        public bool ReceiveGoods(ProductType product, int quantity, Position fromLocation)
        {
            if (CurrentWarehouseLoad + quantity > WarehouseCapacity)
            {
                // Склад переполнен
                return false;
            }

            if (!WarehouseStock.ContainsKey(product))
                WarehouseStock[product] = 0;

            WarehouseStock[product] += quantity;

            // Запись в историю
            DeliveryHistory.Add(new DeliveryRecord
            {
                Timestamp = DateTime.Now,
                Product = product,
                Quantity = quantity,
                Type = DeliveryType.Incoming,
                FromLocation = fromLocation,
                ToLocation = null
            });

            return true;
        }

        /// <summary>
        /// Создать заказ на доставку
        /// </summary>
        public DeliveryOrder? CreateDeliveryOrder(ProductType product, int quantity, Position destination, Building destinationBuilding)
        {
            if (!WarehouseStock.ContainsKey(product) || WarehouseStock[product] < quantity)
            {
                // Недостаточно товара на складе
                return null;
            }

            var order = new DeliveryOrder
            {
                Id = Guid.NewGuid(),
                Product = product,
                Quantity = quantity,
                Destination = destination,
                DestinationBuilding = destinationBuilding,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.Now
            };

            // Резервируем товар
            WarehouseStock[product] -= quantity;

            // Добавляем в активные заказы
            if (!ActiveOrders.ContainsKey(product))
                ActiveOrders[product] = new List<DeliveryOrder>();
            ActiveOrders[product].Add(order);

            return order;
        }

        /// <summary>
        /// Обработать логистику (вызывается каждый игровой тик)
        /// </summary>
        public void ProcessLogistics()
        {
            // 1. Проверить и выполнить готовые заказы
            ProcessPendingOrders();

            // 2. Оптимизировать складские запасы
            OptimizeWarehouse();

            // 3. Обслужить транспорт
            ServiceVehicles();

            // 4. Обновить эффективность
            UpdateEfficiency();
        }

        private void ProcessPendingOrders()
        {
            foreach (var productOrders in ActiveOrders.ToList())
            {
                foreach (var order in productOrders.Value.ToList())
                {
                    if (order.Status == OrderStatus.Pending)
                    {
                        // Пытаемся найти транспорт для доставки
                        var vehicle = FindAvailableVehicle(order);
                        if (vehicle != null)
                        {
                            // Начинаем доставку
                            order.Status = OrderStatus.InTransit;
                            order.AssignedVehicle = vehicle;
                            order.StartedAt = DateTime.Now;

                            // Запись в историю
                            DeliveryHistory.Add(new DeliveryRecord
                            {
                                Timestamp = DateTime.Now,
                                Product = order.Product,
                                Quantity = order.Quantity,
                                Type = DeliveryType.Outgoing,
                                FromLocation = null,
                                ToLocation = order.Destination
                            });
                        }
                    }
                    else if (order.Status == OrderStatus.InTransit)
                    {
                        // Проверяем, завершена ли доставка
                        if ((DateTime.Now - order.StartedAt)!.Value.TotalMinutes > 10) // Упрощенная логика
                        {
                            CompleteOrder(order);
                        }
                    }
                }
            }
        }

        private Transport? FindAvailableVehicle(DeliveryOrder order)
        {
            // Простая логика подбора транспорта
            foreach (var vehicle in AvailableVehicles)
            {
                if (vehicle is LogisticsVehicle logisticsVehicle)
                {
                    if (logisticsVehicle.CurrentCargo == 0 &&
                        logisticsVehicle.MaxCargo >= order.Quantity * 10) // Предполагаем вес
                    {
                        return vehicle;
                    }
                }
            }
            return null;
        }

        private void CompleteOrder(DeliveryOrder order)
        {
            order.Status = OrderStatus.Delivered;
            order.CompletedAt = DateTime.Now;

            // Освобождаем транспорт
            if (order.AssignedVehicle is LogisticsVehicle vehicle)
            {
                vehicle.CurrentCargo = 0;
            }

            // Удаляем из активных заказов
            ActiveOrders[order.Product].Remove(order);
        }

        private void OptimizeWarehouse()
        {
            // Простая логика оптимизации
            // Если какой-то товар заканчивается, создаем заказ на пополнение
            foreach (var stock in WarehouseStock)
            {
                if (stock.Value < 50) // Минимальный запас
                {
                    // Здесь можно было бы создать заказ на фабрику
                    // В реальной игре здесь была бы связь с промышленными зданиями
                }
            }
        }

        private void ServiceVehicles()
        {
            // Обслуживание транспорта (заправка, ремонт)
            foreach (var vehicle in AvailableVehicles)
            {
                if (vehicle is LogisticsVehicle logisticsVehicle)
                {
                    logisticsVehicle.MaintenanceCheck();
                }
            }
        }

        private void UpdateEfficiency()
        {
            // Расчет эффективности на основе выполненных заказов
            int totalOrders = DeliveryHistory.Count(d => d.Type == DeliveryType.Outgoing);
            int completedOrders = ActiveOrders.Values
                .SelectMany(orders => orders)
                .Count(o => o.Status == OrderStatus.Delivered);

            if (totalOrders > 0)
            {
                LogisticsEfficiency = (completedOrders * 100) / totalOrders;
            }
        }

        /// <summary>
        /// Добавить транспорт в парк
        /// </summary>
        public void AddVehicle(LogisticsVehicle vehicle)
        {
            AvailableVehicles.Add(vehicle);
        }

        /// <summary>
        /// Улучшить склад (увеличить вместимость)
        /// </summary>
        public void UpgradeWarehouse(int additionalCapacity = 1000)
        {
            WarehouseCapacity += additionalCapacity;
        }

        /// <summary>
        /// Получить статистику логистического центра
        /// </summary>
        public LogisticsStatistics GetStatistics()
        {
            return new LogisticsStatistics
            {
                TotalStock = CurrentWarehouseLoad,
                AvailableCapacity = WarehouseCapacity - CurrentWarehouseLoad,
                PendingOrders = ActiveOrders.Values.SelectMany(o => o)
                                   .Count(o => o.Status == OrderStatus.Pending),
                ActiveDeliveries = ActiveOrders.Values.SelectMany(o => o)
                                    .Count(o => o.Status == OrderStatus.InTransit),
                Efficiency = LogisticsEfficiency,
                VehicleCount = AvailableVehicles.Count
            };
        }
    }

    // Вспомогательные классы

    /// <summary>
    /// Заказ на доставку
    /// </summary>
    public class DeliveryOrder
    {
        public Guid Id { get; set; }
        public ProductType Product { get; set; }
        public int Quantity { get; set; }
        public Position Destination { get; set; }
        public Building DestinationBuilding { get; set; }
        public OrderStatus Status { get; set; }
        public Transport? AssignedVehicle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Запись о доставке
    /// </summary>
    public class DeliveryRecord
    {
        public DateTime Timestamp { get; set; }
        public ProductType Product { get; set; }
        public int Quantity { get; set; }
        public DeliveryType Type { get; set; }
        public Position? FromLocation { get; set; }
        public Position? ToLocation { get; set; }
    }

    /// <summary>
    /// Статистика логистического центра
    /// </summary>
    public class LogisticsStatistics
    {
        public int TotalStock { get; set; }
        public int AvailableCapacity { get; set; }
        public int PendingOrders { get; set; }
        public int ActiveDeliveries { get; set; }
        public int Efficiency { get; set; }
        public int VehicleCount { get; set; }
    }

    public enum OrderStatus
    {
        Pending,      // Ожидает обработки
        InTransit,    // В пути
        Delivered,    // Доставлено
        Cancelled     // Отменено
    }

    public enum DeliveryType
    {
        Incoming,     // Поступление на склад
        Outgoing      // Отправка со склада
    }
}