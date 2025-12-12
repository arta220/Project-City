/*using Domain.Enums;
using Domain.Infrastructure;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class TileModelPathTests
    {
        [TestMethod]
        public void CanPlace_PedestrianPath_OnValidTerrain()
        {
            // Arrange
            var tile = new TileModel
            {
                Position = new Position(0, 0),
                Terrain = TerrainType.Plain
            };
            var path = new PedestrianPath();

            // Act
            var canPlace = tile.CanPlace(path);

            // Assert
            Assert.IsTrue(canPlace);
        }

        [TestMethod]
        public void CanPlace_Path_OnWater_ReturnsFalse()
        {
            // Arrange
            var tile = new TileModel
            {
                Position = new Position(0, 0),
                Terrain = TerrainType.Water
            };
            var path = new PedestrianPath();

            // Act
            var canPlace = tile.CanPlace(path);

            // Assert
            Assert.IsFalse(canPlace);
        }

        [TestMethod]
        public void CanPlace_Path_OnMountain_ReturnsFalse()
        {
            // Arrange
            var tile = new TileModel
            {
                Position = new Position(0, 0),
                Terrain = TerrainType.Mountain
            };
            var path = new BicyclePath();

            // Act
            var canPlace = tile.CanPlace(path);

            // Assert
            Assert.IsFalse(canPlace);
        }
    }
}
*/