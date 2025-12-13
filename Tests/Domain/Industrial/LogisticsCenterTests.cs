using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Buildings.Logistics;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;
using Domain.Transports;
using Domain.Transports.Ground;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain.Buildings.Logistics
{
    [TestClass]
    public class LogisticsCenterTests
    {
        private LogisticsCenter _logisticsCenter;
        private Area _testArea;
        private Position _testPosition;

        [TestInitialize]
        public void TestInitialize()
        {
            // ИНИЦИАЛИЗАЦИЯ ОБЪЕКТОВ ДО КАЖДОГО ТЕСТА
            // Используем конструктор Area с 2 параметрами, как в CardboardFactoryTests
            _testArea = new Area(5, 5);
            _testPosition = new Position(150, 150);
            _logisticsCenter = new LogisticsCenter(3, 50, _testArea);
        }

        [TestMethod]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Assert
            Assert.AreEqual(3, _logisticsCenter.Floors);
            Assert.AreEqual(50, _logisticsCenter.MaxOccupancy);
            Assert.AreEqual(5000, _logisticsCenter.WarehouseCapacity);
            Assert.AreEqual(75, _logisticsCenter.LogisticsEfficiency);
        }

        [TestMethod]
        public void Initialize_ShouldSetUpVacanciesCorrectly()
        {
            // Assert
            Assert.AreEqual(2, _logisticsCenter.Vacancies[CitizenProfession.LogisticsManager]);
            Assert.AreEqual(15, _logisticsCenter.Vacancies[CitizenProfession.WarehouseWorker]);
            Assert.AreEqual(8, _logisticsCenter.Vacancies[CitizenProfession.TruckDriver]);
            Assert.AreEqual(1, _logisticsCenter.Vacancies[CitizenProfession.Dispatcher]);
        }

        [TestMethod]
        public void Initialize_ShouldSetUpMaxAgesCorrectly()
        {
            // Assert
            Assert.AreEqual(65, _logisticsCenter.MaxAges[CitizenProfession.LogisticsManager]);
            Assert.AreEqual(55, _logisticsCenter.MaxAges[CitizenProfession.WarehouseWorker]);
            Assert.AreEqual(60, _logisticsCenter.MaxAges[CitizenProfession.TruckDriver]);
            Assert.AreEqual(62, _logisticsCenter.MaxAges[CitizenProfession.Dispatcher]);
        }

        [TestMethod]
        public void InitializeBasicStock_ShouldCreateInitialStock()
        {
            // Assert
            Assert.AreEqual(100, _logisticsCenter.WarehouseStock[ProductType.FoodContainer]);
            Assert.AreEqual(200, _logisticsCenter.WarehouseStock[ProductType.CardboardBox]);
            Assert.AreEqual(150, _logisticsCenter.WarehouseStock[ProductType.PlasticBottle]);
        }

        [TestMethod]
        public void ReceiveGoods_WhenWarehouseHasCapacity_ShouldAcceptGoods()
        {
            // Arrange
            var initialLoad = _logisticsCenter.CurrentWarehouseLoad;
            var product = ProductType.FoodContainer;
            var quantity = 50;

            // Act
            var result = _logisticsCenter.ReceiveGoods(product, quantity, _testPosition);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(initialLoad + quantity, _logisticsCenter.CurrentWarehouseLoad);
            Assert.AreEqual(150, _logisticsCenter.WarehouseStock[product]); // 100 initial + 50
        }

        [TestMethod]
        public void ReceiveGoods_WhenWarehouseFull_ShouldRejectGoods()
        {
            // Arrange
            var product = ProductType.FoodContainer;
            var hugeQuantity = 10000; // Больше чем вместимость

            // Act
            var result = _logisticsCenter.ReceiveGoods(product, hugeQuantity, _testPosition);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(100, _logisticsCenter.WarehouseStock[product]); // Не изменилось
        }

        [TestMethod]
        public void ReceiveGoods_ShouldAddToDeliveryHistory()
        {
            // Arrange
            var product = ProductType.FoodContainer;
            var quantity = 50;
            var initialHistoryCount = _logisticsCenter.DeliveryHistory.Count;

            // Act
            _logisticsCenter.ReceiveGoods(product, quantity, _testPosition);

            // Assert
            Assert.AreEqual(initialHistoryCount + 1, _logisticsCenter.DeliveryHistory.Count);
            var lastRecord = _logisticsCenter.DeliveryHistory.Last();
            Assert.AreEqual(product, lastRecord.Product);
            Assert.AreEqual(quantity, lastRecord.Quantity);
            Assert.AreEqual(DeliveryType.Incoming, lastRecord.Type);
            Assert.AreEqual(_testPosition, lastRecord.FromLocation);
        }

        [TestMethod]
        public void CreateDeliveryOrder_WhenSufficientStock_ShouldCreateOrder()
        {
            // Arrange
            var product = ProductType.FoodContainer;
            var quantity = 50;
            var destination = new Position(300, 300);
            var building = new TestBuilding();

            // Act
            var order = _logisticsCenter.CreateDeliveryOrder(product, quantity, destination, building);

            // Assert
            Assert.IsNotNull(order);
            Assert.AreEqual(product, order.Product);
            Assert.AreEqual(quantity, order.Quantity);
            Assert.AreEqual(destination, order.Destination);
            Assert.AreEqual(OrderStatus.Pending, order.Status);
            Assert.AreEqual(50, _logisticsCenter.WarehouseStock[product]); // 100 - 50
            Assert.IsTrue(_logisticsCenter.ActiveOrders[product].Contains(order));
        }

        [TestMethod]
        public void CreateDeliveryOrder_WhenInsufficientStock_ShouldReturnNull()
        {
            // Arrange
            var product = ProductType.FoodContainer;
            var hugeQuantity = 1000; // Больше чем есть на складе
            var destination = new Position(300, 300);
            var building = new TestBuilding();

            // Act
            var order = _logisticsCenter.CreateDeliveryOrder(product, hugeQuantity, destination, building);

            // Assert
            Assert.IsNull(order);
            Assert.AreEqual(100, _logisticsCenter.WarehouseStock[product]); // Не изменилось
        }

        [TestMethod]
        public void UpgradeWarehouse_ShouldIncreaseCapacity()
        {
            // Arrange
            var initialCapacity = _logisticsCenter.WarehouseCapacity;

            // Act
            _logisticsCenter.UpgradeWarehouse(1000);

            // Assert
            Assert.AreEqual(initialCapacity + 1000, _logisticsCenter.WarehouseCapacity);
        }

        [TestMethod]
        public void GetStatistics_ShouldReturnCorrectValues()
        {
            // Act
            var stats = _logisticsCenter.GetStatistics();

            // Assert
            Assert.AreEqual(_logisticsCenter.CurrentWarehouseLoad, stats.TotalStock);
            Assert.AreEqual(_logisticsCenter.WarehouseCapacity - _logisticsCenter.CurrentWarehouseLoad, stats.AvailableCapacity);
            Assert.AreEqual(_logisticsCenter.AvailableVehicles.Count, stats.VehicleCount);
            Assert.AreEqual(_logisticsCenter.LogisticsEfficiency, stats.Efficiency);
        }

        [TestMethod]
        public void ProcessLogistics_ShouldNotThrowExceptions()
        {
            // Act & Assert
            try
            {
                _logisticsCenter.ProcessLogistics();
            }
            catch (Exception ex)
            {
                Assert.Fail($"ProcessLogistics threw an exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void CurrentWarehouseLoad_ShouldCalculateCorrectly()
        {
            // Arrange
            var expectedLoad = _logisticsCenter.WarehouseStock.Values.Sum();

            // Act
            var actualLoad = _logisticsCenter.CurrentWarehouseLoad;

            // Assert
            Assert.AreEqual(expectedLoad, actualLoad);
        }

        [TestMethod]
        public void DeliveryOrder_PropertiesShouldBeSettable()
        {
            // Arrange
            var order = new DeliveryOrder();
            var testPosition = new Position(100, 100);
            var testBuilding = new TestBuilding();
            var testDateTime = DateTime.Now;

            // Act
            order.Product = ProductType.FoodContainer;
            order.Quantity = 50;
            order.Destination = testPosition;
            order.DestinationBuilding = testBuilding;
            order.Status = OrderStatus.Pending;
            order.CreatedAt = testDateTime;
            order.StartedAt = testDateTime.AddMinutes(5);
            order.CompletedAt = testDateTime.AddMinutes(30);

            // Assert
            Assert.AreEqual(ProductType.FoodContainer, order.Product);
            Assert.AreEqual(50, order.Quantity);
            Assert.AreEqual(testPosition, order.Destination);
            Assert.AreEqual(testBuilding, order.DestinationBuilding);
            Assert.AreEqual(OrderStatus.Pending, order.Status);
            Assert.AreEqual(testDateTime, order.CreatedAt);
            Assert.AreEqual(testDateTime.AddMinutes(5), order.StartedAt);
            Assert.AreEqual(testDateTime.AddMinutes(30), order.CompletedAt);
        }

        [TestMethod]
        public void DeliveryRecord_PropertiesShouldBeSettable()
        {
            // Arrange
            var record = new DeliveryRecord();
            var testDateTime = DateTime.Now;
            var fromPosition = new Position(100, 100);
            var toPosition = new Position(200, 200);

            // Act
            record.Timestamp = testDateTime;
            record.Product = ProductType.FoodContainer;
            record.Quantity = 100;
            record.Type = DeliveryType.Outgoing;
            record.FromLocation = fromPosition;
            record.ToLocation = toPosition;

            // Assert
            Assert.AreEqual(testDateTime, record.Timestamp);
            Assert.AreEqual(ProductType.FoodContainer, record.Product);
            Assert.AreEqual(100, record.Quantity);
            Assert.AreEqual(DeliveryType.Outgoing, record.Type);
            Assert.AreEqual(fromPosition, record.FromLocation);
            Assert.AreEqual(toPosition, record.ToLocation);
        }

        [TestMethod]
        public void LogisticsStatistics_PropertiesShouldBeSettable()
        {
            // Arrange
            var stats = new LogisticsStatistics();

            // Act
            stats.TotalStock = 1000;
            stats.AvailableCapacity = 4000;
            stats.PendingOrders = 5;
            stats.ActiveDeliveries = 3;
            stats.Efficiency = 85;
            stats.VehicleCount = 10;

            // Assert
            Assert.AreEqual(1000, stats.TotalStock);
            Assert.AreEqual(4000, stats.AvailableCapacity);
            Assert.AreEqual(5, stats.PendingOrders);
            Assert.AreEqual(3, stats.ActiveDeliveries);
            Assert.AreEqual(85, stats.Efficiency);
            Assert.AreEqual(10, stats.VehicleCount);
        }

        // Вспомогательные классы для тестирования

        private class TestBuilding : Building
        {
            public TestBuilding() : base(1, 10, new Area(0, 0)) { }
        }
    }
}