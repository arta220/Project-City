using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Enums;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class CommercialBuildingsTests
    {
        [TestMethod]
        public void Pharmacy_ShouldHaveCorrectProperties()
        {
            // Arrange
            var area = new Area(1, 1);

            // Act
            var pharmacy = new Pharmacy(area);

            // Assert
            Assert.AreEqual(CommercialType.Pharmacy, pharmacy.CommercialType);
            Assert.AreEqual(2, pharmacy.WorkerCount);
            Assert.AreEqual(6, pharmacy.MaxOccupancy); // 2 workers * 3 = 6
            Assert.AreEqual(6, pharmacy.MaxQueueLength); // Equal to MaxOccupancy
            Assert.AreEqual(10, pharmacy.ServiceTimeInTicks);
        }

        [TestMethod]
        public void CommercialBuilding_CanAcceptMoreVisitors_WhenUnderCapacity()
        {
            // Arrange
            var area = new Area(1, 1);
            var pharmacy = new Pharmacy(area);

            // Act & Assert
            Assert.IsTrue(pharmacy.CanAcceptMoreVisitors);
            Assert.AreEqual(0, pharmacy.CurrentVisitors);
        }

        [TestMethod]
        public void TryJoinQueue_ShouldAddToQueue_WhenCanAcceptMoreVisitors()
        {
            // Arrange
            var area = new Area(1, 1);
            var pharmacy = new Pharmacy(area);

            // Act
            var result = pharmacy.TryJoinQueue();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, pharmacy.CurrentQueue);
            Assert.AreEqual(0, pharmacy.CurrentVisitors); // Гражданин в очереди, но не в здании
        }

        [TestMethod]
        public void ProcessQueue_ShouldMoveCitizensFromQueueToService()
        {
            // Arrange
            var area = new Area(1, 1);
            var pharmacy = new Pharmacy(area);

            // Добавляем в очередь
            pharmacy.TryJoinQueue();
            pharmacy.TryJoinQueue();

            // Act
            pharmacy.ProcessQueue(); // Перемещаем из очереди в обслуживание

            // Assert
            Assert.AreEqual(2, pharmacy.CurrentVisitors);
            Assert.AreEqual(0, pharmacy.CurrentQueue); // Очередь опустела
        }

        [TestMethod]
        public void TryEnter_ShouldWorkCorrectly()
        {
            // Arrange
            var area = new Area(1, 1);
            var pharmacy = new Pharmacy(area); // MaxOccupancy = 6

            // Act & Assert
            // Добавляем посетителей
            for (int i = 0; i < 6; i++)
            {
                Assert.IsTrue(pharmacy.TryEnter());
            }

            // Пытаемся добавить еще одного (должно не получиться)
            Assert.IsFalse(pharmacy.TryEnter());
            Assert.AreEqual(6, pharmacy.CurrentVisitors); // Максимум достигнут
        }

        [TestMethod]
        public void Leave_ShouldDecreaseCurrentVisitors()
        {
            // Arrange
            var area = new Area(1, 1);
            var pharmacy = new Pharmacy(area);

            // Добавляем посетителей
            pharmacy.TryEnter();
            pharmacy.TryEnter();
            Assert.AreEqual(2, pharmacy.CurrentVisitors);

            // Act
            pharmacy.Leave();

            // Assert
            Assert.AreEqual(1, pharmacy.CurrentVisitors);

            // Проверяем на крайний случай
            pharmacy.Leave();
            Assert.AreEqual(0, pharmacy.CurrentVisitors);

            // Еще один вызов не должен уйти в отрицательные значения
            pharmacy.Leave();
            Assert.AreEqual(0, pharmacy.CurrentVisitors);
        }

        [TestMethod]
        public void LeaveQueue_ShouldDecreaseCurrentQueue()
        {
            // Arrange
            var area = new Area(1, 1);
            var pharmacy = new Pharmacy(area);

            // Добавляем в очередь
            pharmacy.TryJoinQueue();
            pharmacy.TryJoinQueue();
            Assert.AreEqual(2, pharmacy.CurrentQueue);

            // Act
            pharmacy.LeaveQueue();

            // Assert
            Assert.AreEqual(1, pharmacy.CurrentQueue);

            // Проверяем на крайний случай
            pharmacy.LeaveQueue();
            Assert.AreEqual(0, pharmacy.CurrentQueue);

            // Еще один вызов не должен уйти в отрицательные значения
            pharmacy.LeaveQueue();
            Assert.AreEqual(0, pharmacy.CurrentQueue);
        }

        [TestMethod]
        public void CommercialBuilding_ShouldImplementIServiceBuilding()
        {
            // Arrange
            var area = new Area(1, 1);
            var pharmacy = new Pharmacy(area);

            // Act & Assert
            Assert.IsInstanceOfType(pharmacy, typeof(IServiceBuilding));
        }

        [TestMethod]
        public void DifferentCommercialBuildings_ShouldHaveDifferentProperties()
        {
            // Arrange & Act
            var pharmacy = new Pharmacy(new Area(1, 1));
            var shop = new Shop(new Area(2, 2));

            // Assert
            Assert.AreEqual(CommercialType.Pharmacy, pharmacy.CommercialType);
            Assert.AreEqual(CommercialType.Shop, shop.CommercialType);

            Assert.AreEqual(2, pharmacy.WorkerCount);
            Assert.AreEqual(3, shop.WorkerCount);

            Assert.AreEqual(6, pharmacy.MaxOccupancy);
            Assert.AreEqual(9, shop.MaxOccupancy); // 3 workers * 3 = 9

            Assert.AreEqual(6, pharmacy.MaxQueueLength);
            Assert.AreEqual(9, shop.MaxQueueLength); // Обычно равно MaxOccupancy
        }

        [TestMethod]
        public void ProcessQueue_ShouldRespectMaxOccupancy()
        {
            // Arrange
            var area = new Area(1, 1);
            var pharmacy = new Pharmacy(area); // MaxOccupancy = 6, MaxQueueLength = 6

            // Заполняем здание частично
            pharmacy.TryEnter(); // 1 посетитель внутри

            // Заполняем очередь
            for (int i = 0; i < 8; i++) // Добавляем больше чем максимум очереди
            {
                pharmacy.TryJoinQueue();
            }

            // Act
            pharmacy.ProcessQueue();

            // Assert
            // Должны переместиться только 5 посетителей из очереди (1 + 5 = 6)
            Assert.AreEqual(6, pharmacy.CurrentVisitors);
            Assert.AreEqual(1, pharmacy.CurrentQueue); // 1 остался в очереди (6 - 5 = 1)
        }

        [TestMethod]
        public void CitizenState_ShouldHaveCommercialStates()
        {
            // Arrange & Act
            var goingToCommercial = CitizenState.GoingToCommercial;

            // Assert
            Assert.AreEqual("GoingToCommercial", goingToCommercial.ToString());
        }
    }
}