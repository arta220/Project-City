using Domain.Common.Enums;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.MapGenerator;

namespace Tests.Services

{
    [TestClass]
    public class MapGeneratorTests
    {
        private MapGenerator CreateGenerator() => new MapGenerator();

        private MapModel GenerateDefaultMap()
        {
            var generator = CreateGenerator();
            return generator.GenerateMap(50, 50);
        }

        [TestMethod]
        public void GenerateMap_CreatesMapWithCorrectSize()
        {
            // Arrange
            int width = 50;
            int height = 50;
            var generator = CreateGenerator();

            // Act
            var map = generator.GenerateMap(width, height);

            // Assert
            Assert.AreEqual(width, map.Width);
            Assert.AreEqual(height, map.Height);
        }

        [TestMethod]
        public void GenerateMap_IsDeterministic_ForSameSize()
        {
            // Arrange
            var generator = CreateGenerator();

            // Act
            var map1 = generator.GenerateMap(50, 50);
            var map2 = generator.GenerateMap(50, 50);

            // Assert
            for (int x = 0; x < map1.Width; x++)
            {
                for (int y = 0; y < map1.Height; y++)
                {
                    var t1 = map1[x, y];
                    var t2 = map2[x, y];

                    Assert.AreEqual(t1.Terrain, t2.Terrain, $"Terrain mismatch at ({x},{y})");
                    Assert.AreEqual(t1.ResourceType, t2.ResourceType, $"ResourceType mismatch at ({x},{y})");
                    Assert.AreEqual(t1.ResourceAmount, t2.ResourceAmount, $"ResourceAmount mismatch at ({x},{y})");
                }
            }
        }

        [TestMethod]
        public void BorderTiles_AreWater()
        {
            // Arrange
            var map = GenerateDefaultMap();
            int w = map.Width;
            int h = map.Height;

            // Act & Assert
            for (int x = 0; x < w; x++)
            {
                Assert.AreEqual(TerrainType.Water, map[x, 0].Terrain);
                Assert.AreEqual(TerrainType.Water, map[x, h - 1].Terrain);
            }

            for (int y = 0; y < h; y++)
            {
                Assert.AreEqual(TerrainType.Water, map[0, y].Terrain);
                Assert.AreEqual(TerrainType.Water, map[w - 1, y].Terrain);
            }
        }

        [TestMethod]
        public void ForestTiles_HaveWoodResource()
        {
            // Arrange
            var map = GenerateDefaultMap();
            bool foundForest = false;

            // Act & Assert
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var tile = map[x, y];

                    if (tile.Terrain == TerrainType.Forest)
                    {
                        foundForest = true;

                        Assert.AreEqual(NaturalResourceType.Wood, tile.ResourceType,
                            $"Forest tile at ({x},{y}) must have Wood.");
                        Assert.IsTrue(tile.ResourceAmount > 0,
                            $"Forest tile at ({x},{y}) must have positive Wood amount.");
                    }
                }
            }

            Assert.IsTrue(foundForest, "Map must contain at least one forest tile.");
        }

        [TestMethod]
        public void MountainTiles_ContainOnlyIronCopperOrNone()
        {
            // Arrange
            var map = GenerateDefaultMap();
            bool foundMountains = false;

            // Act & Assert
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var tile = map[x, y];

                    if (tile.Terrain != TerrainType.Mountain)
                        continue;

                    foundMountains = true;

                    Assert.IsTrue(
                        tile.ResourceType == NaturalResourceType.None ||
                        tile.ResourceType == NaturalResourceType.Iron ||
                        tile.ResourceType == NaturalResourceType.Copper,
                        $"Mountain tile at ({x},{y}) must contain Iron/Copper/None, but was {tile.ResourceType}");

                    Assert.AreNotEqual(NaturalResourceType.Oil, tile.ResourceType);
                    Assert.AreNotEqual(NaturalResourceType.Gas, tile.ResourceType);
                    Assert.AreNotEqual(NaturalResourceType.Wood, tile.ResourceType);
                }
            }

            Assert.IsTrue(foundMountains, "Map must contain at least one mountain tile.");
        }

        [TestMethod]
        public void WaterTiles_DoNotContainResources()
        {
            // Arrange
            var map = GenerateDefaultMap();

            // Act & Assert
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var tile = map[x, y];

                    if (tile.Terrain == TerrainType.Water)
                    {
                        Assert.AreEqual(NaturalResourceType.None, tile.ResourceType,
                            $"Water tile at ({x},{y}) must not have resource.");
                        Assert.AreEqual(0, tile.ResourceAmount,
                            $"Water tile at ({x},{y}) must have zero resource amount.");
                    }
                }
            }
        }
    }
}