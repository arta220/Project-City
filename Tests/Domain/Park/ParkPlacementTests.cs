//using Domain.Base;
//using Domain.Enums;
//using Domain.Map;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Tests.Domain
//{
//    [TestClass]
//    public class ParkPlacementTests
//    {
//        [TestMethod]
//        public void CanPlace_Park_OnValidTerrain()
//        {
//            // Arrange
//            var tile = new TileModel
//            {
//                Position = new Position(0, 0),
//                Terrain = TerrainType.Plain
//            };
//            var park = new Park(new Area(2, 2), ParkType.UrbanPark);

//            // Act
//            var canPlace = tile.CanPlace(park);

//            // Assert
//            Assert.IsTrue(canPlace);
//        }

//        [TestMethod]
//        public void Park_TreesCount_VariesByTypeAndSize()
//        {
//            // Arrange & Act
//            var smallUrban = new Park(new Area(2, 2), ParkType.UrbanPark);      // 4 * 3 = 12
//            var largeBotanical = new Park(new Area(5, 5), ParkType.BotanicalGarden); // 25 * 5 = 125
//            var playground = new Park(new Area(1, 1), ParkType.Playground);     // 1 * 2 = 2

//            // Assert
//            Assert.AreEqual(12, smallUrban.TreesCount);
//            Assert.AreEqual(125, largeBotanical.TreesCount);
//            Assert.AreEqual(2, playground.TreesCount);
//        }
//    }
//}