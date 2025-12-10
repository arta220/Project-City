using Domain.Base;
using Domain.Common.Enums;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class ParkTerrainTests
    {
        [TestMethod]
        public void Park_TerrainType_MatchesParkType()
        {
            Assert.AreEqual(TerrainType.UrbanPark, new Park(new Area(1, 1), ParkType.UrbanPark).TerrainType);
            Assert.AreEqual(TerrainType.BotanicalGarden, new Park(new Area(1, 1), ParkType.BotanicalGarden).TerrainType);
            Assert.AreEqual(TerrainType.Playground, new Park(new Area(1, 1), ParkType.Playground).TerrainType);
            Assert.AreEqual(TerrainType.Square, new Park(new Area(1, 1), ParkType.Square).TerrainType);
            Assert.AreEqual(TerrainType.RecreationArea, new Park(new Area(1, 1), ParkType.RecreationArea).TerrainType);
        }

        [TestMethod]
        public void MapModel_OverridesTerrain_ForParkArea_AndRestoresOnRemove()
        {
            var map = new MapModel(width: 5, height: 5);
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    map[x, y] = new TileModel
                    {
                        Position = new Position(x, y),
                        Terrain = TerrainType.Plain,
                        Height = 0
                    };
                }
            }

            var park = new Park(new Area(2, 2), ParkType.UrbanPark);
            var placement = new Placement(new Position(1, 1), park.Area);

            var setOk = map.TrySetMapObject(park, placement);
            Assert.IsTrue(setOk);

            foreach (var pos in placement.GetAllPositions())
            {
                Assert.AreEqual(park.TerrainType, map[pos].Terrain);
                Assert.AreSame(park, map[pos].MapObject);
            }

            var removeOk = map.TryRemoveMapObject(placement);
            Assert.IsTrue(removeOk);

            foreach (var pos in placement.GetAllPositions())
            {
                Assert.AreEqual(TerrainType.Plain, map[pos].Terrain);
                Assert.IsNull(map[pos].MapObject);
            }
        }
    }
}

