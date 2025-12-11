using Domain.Buildings;
using Domain.Citizens;
using Domain.Enums;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class ServiceBuildingIntegrationTests
    {
        // Вспомогательный тестовый класс для тестирования абстрактного CommercialBuilding
        private class TestCommercialBuilding : CommercialBuilding
        {
            public override CommercialType CommercialType => CommercialType.Shop;

            public TestCommercialBuilding(Area area, int serviceTime = 10, int maxQueue = 6, int workerCount = 2)
                : base(area, serviceTime, maxQueue, workerCount)
            {
            }
        }

        [TestMethod]
        public void CommercialBuilding_ShouldNotExceedMaxQueue()
        {
            // Arrange
            var area = new Area(1, 1);
            var building = new TestCommercialBuilding(area, maxQueue: 6, workerCount: 2);

            // Act - добавляем больше граждан чем максимальная очередь
            for (int i = 0; i < 10; i++)
            {
                building.TryJoinQueue();
            }

            // Assert
            // Максимальная очередь = 6, поэтому CurrentQueue не должен превышать 6
            Assert.AreEqual(6, building.CurrentQueue);
            Assert.IsFalse(building.CanAcceptMoreVisitors);
        }

        [TestMethod]
        public void CommercialBuilding_CanAcceptMoreVisitors_ShouldWorkCorrectly()
        {
            // Arrange
            var area = new Area(1, 1);
            var building = new TestCommercialBuilding(area, maxQueue: 4, workerCount: 2); // MaxOccupancy = 6 (2*3)

            // Act & Assert - Проверяем различные состояния
            // 1. Пустое здание - должно принимать посетителей
            Assert.IsTrue(building.CanAcceptMoreVisitors);

            // 2. Заполняем очередь
            for (int i = 0; i < 4; i++)
            {
                building.TryJoinQueue();
            }
            Assert.IsFalse(building.CanAcceptMoreVisitors); // Очередь заполнена

            // 3. Освобождаем очередь
            building.LeaveQueue();
            Assert.IsTrue(building.CanAcceptMoreVisitors);
        }

        [TestMethod]
        public void CommercialBuilding_TryEnter_ShouldRespectMaxOccupancy()
        {
            // Arrange
            var area = new Area(1, 1);
            var building = new TestCommercialBuilding(area, workerCount: 1); // MaxOccupancy = 3 (1*3)

            // Act & Assert
            // Заполняем здание
            Assert.IsTrue(building.TryEnter()); // 1
            Assert.IsTrue(building.TryEnter()); // 2
            Assert.IsTrue(building.TryEnter()); // 3
            Assert.IsFalse(building.TryEnter()); // 4 - превышение лимита
            Assert.AreEqual(3, building.CurrentVisitors);
        }

        [TestMethod]
        public void CommercialBuilding_CalculateMaxOccupancy_ShouldDependOnWorkerCount()
        {
            // Arrange & Act
            var area = new Area(1, 1);

            // Test case 1: 1 worker
            var building1 = new TestCommercialBuilding(area, workerCount: 1);
            Assert.AreEqual(3, building1.MaxOccupancy);

            // Test case 2: 3 workers
            var building3 = new TestCommercialBuilding(area, workerCount: 3);
            Assert.AreEqual(9, building3.MaxOccupancy);

            // Test case 3: 0 workers (edge case)
            var building0 = new TestCommercialBuilding(area, workerCount: 0);
            Assert.AreEqual(0, building0.MaxOccupancy);
        }

        [TestMethod]
        public void CommercialType_ShouldHaveAllExpectedValues()
        {
            // Arrange & Act
            var types = new[]
            {
                CommercialType.Pharmacy,
                CommercialType.Shop,
                CommercialType.Supermarket,
                CommercialType.Cafe,
                CommercialType.Restaurant,
                CommercialType.GasStation
            };

            // Assert
            Assert.AreEqual(6, types.Length);
            CollectionAssert.AllItemsAreUnique(types);

            // Дополнительная проверка значений enum
            foreach (var type in types)
            {
                Assert.IsTrue(Enum.IsDefined(typeof(CommercialType), type));
            }
        }

        [TestMethod]
        public void CommercialBuilding_Properties_ShouldBeCorrectlyInitialized()
        {
            // Arrange
            var area = new Area(2, 3);
            int serviceTime = 15;
            int maxQueue = 8;
            int workerCount = 4;

            // Act
            var building = new TestCommercialBuilding(area, serviceTime, maxQueue, workerCount);

            // Assert
            Assert.AreEqual(serviceTime, building.ServiceTimeInTicks);
            Assert.AreEqual(maxQueue, building.MaxQueueLength);
            Assert.AreEqual(workerCount, building.WorkerCount);
            Assert.AreEqual(area, building.Area);
            Assert.AreEqual(12, building.MaxOccupancy); // 4 * 3
            Assert.AreEqual(0, building.CurrentVisitors);
            Assert.AreEqual(0, building.CurrentQueue);
        }
    }
}