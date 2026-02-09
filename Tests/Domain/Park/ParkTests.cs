//using Domain.Base;
//using Domain.Enums;
//using Domain.Map;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Tests.Domain
//{
//    [TestClass]
//    public class ParkTests
//    {
//        [TestMethod]
//        public void Park_Creation_SetsCorrectProperties()
//        {
//            // Arrange
//            var area = new Area(3, 3);
//            var parkType = ParkType.UrbanPark;

//            // Act
//            var park = new Park(area, parkType);

//            // Assert
//            Assert.AreEqual(parkType, park.Type);
//            Assert.AreEqual(area, park.Area);
//            Assert.AreEqual(3, park.Area.Width);
//            Assert.AreEqual(3, park.Area.Height);
//        }

//        [TestMethod]
//        public void CalculateTreesCount_BotanicalGarden_ReturnsCorrectCount()
//        {
//            // Arrange
//            var area = new Area(4, 4);
//            var parkType = ParkType.BotanicalGarden;

//            // Act
//            var park = new Park(area, parkType);

//            // Assert
//            Assert.AreEqual(4 * 4 * 5, park.TreesCount); // 16 * 5 = 80
//        }

//        [TestMethod]
//        public void CalculateTreesCount_Square_ReturnsCorrectCount()
//        {
//            // Arrange
//            var area = new Area(2, 3);
//            var parkType = ParkType.Square;

//            // Act
//            var park = new Park(area, parkType);

//            // Assert
//            Assert.AreEqual(2 * 3 * 4, park.TreesCount); // 6 * 4 = 24
//        }

//        [TestMethod]
//        public void CalculateTreesCount_UrbanPark_ReturnsCorrectCount()
//        {
//            // Arrange
//            var area = new Area(3, 3);
//            var parkType = ParkType.UrbanPark;

//            // Act
//            var park = new Park(area, parkType);

//            // Assert
//            Assert.AreEqual(3 * 3 * 3, park.TreesCount); // 9 * 3 = 27
//        }

//        [TestMethod]
//        public void CalculateTreesCount_Playground_ReturnsDefaultCount()
//        {
//            // Arrange
//            var area = new Area(1, 1);
//            var parkType = ParkType.Playground;

//            // Act
//            var park = new Park(area, parkType);

//            // Assert
//            Assert.AreEqual(1 * 1 * 2, park.TreesCount); // 1 * 2 = 2
//        }

//        [TestMethod]
//        public void CalculateTreesCount_RecreationArea_ReturnsDefaultCount()
//        {
//            // Arrange
//            var area = new Area(2, 3);
//            var parkType = ParkType.RecreationArea;

//            // Act
//            var park = new Park(area, parkType);

//            // Assert
//            Assert.AreEqual(2 * 3 * 2, park.TreesCount); // 6 * 2 = 12
//        }

//        [TestMethod]
//        public void Park_IsMapObject()
//        {
//            // Arrange & Act
//            var park = new Park(new Area(2, 2), ParkType.UrbanPark);

//            // Assert
//            Assert.IsInstanceOfType(park, typeof(MapObject));
//        }
//    }
//}