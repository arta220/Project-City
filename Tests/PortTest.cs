using Domain.Buildings;
using Domain.Common.Base;
using Domain.Map;
using Domain.Transports;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void Position_StoresCoordinates()
        {
            var position = new Position(10, 20);

            Assert.AreEqual(10, position.X);
            Assert.AreEqual(20, position.Y);
        }

        [TestMethod]
        public void Position_EqualPositions_AreEqual()
        {
            var first = new Position(5, 7);
            var second = new Position(5, 7);

            Assert.IsTrue(first == second);
        }

        [TestMethod]
        public void Position_DifferentPositions_AreNotEqual()
        {
            var first = new Position(5, 7);
            var second = new Position(5, 8);

            Assert.IsTrue(first != second);
        }

        [TestMethod]
        public void Area_StoresWidthAndHeight()
        {
            var area = new Area(4, 6);

            Assert.AreEqual(4, area.Width);
            Assert.AreEqual(6, area.Height);
        }

        [TestMethod]
        public void Placement_StoresPositionAndArea()
        {
            var position = new Position(10, 15);
            var area = new Area(3, 4);

            var placement = new Placement(position, area);

            Assert.AreEqual(position, placement.Position);
            Assert.AreEqual(area, placement.Area);
        }

        [TestMethod]
        public void Placement_CalculatesLeftAndTop()
        {
            var placement = new Placement(
                new Position(10, 15),
                new Area(3, 4));

            Assert.AreEqual(10, placement.Left);
            Assert.AreEqual(15, placement.Top);
        }

        [TestMethod]
        public void Placement_CalculatesRightAndBottom()
        {
            var placement = new Placement(
                new Position(10, 15),
                new Area(3, 4));

            Assert.AreEqual(13, placement.Right);
            Assert.AreEqual(19, placement.Bottom);
        }

        [TestMethod]
        public void Placement_ContainsInnerPosition()
        {
            var placement = new Placement(
                new Position(10, 15),
                new Area(5, 5));

            var point = new Position(12, 17);

            Assert.IsTrue(placement.Contains(point));
        }

        [TestMethod]
        public void Placement_IntersectsOverlappingAreas()
        {
            var first = new Placement(
                new Position(10, 10),
                new Area(5, 5));

            var second = new Placement(
                new Position(12, 12),
                new Area(5, 5));

            Assert.IsTrue(first.Intersects(second));
        }

        [TestMethod]
        public void Placement_ReturnsAllPositions()
        {
            var placement = new Placement(
                new Position(2, 3),
                new Area(2, 2));

            var positions = placement.GetAllPositions().ToList();

            Assert.AreEqual(4, positions.Count);
            Assert.IsTrue(positions.Contains(new Position(2, 3)));
            Assert.IsTrue(positions.Contains(new Position(3, 3)));
            Assert.IsTrue(positions.Contains(new Position(2, 4)));
            Assert.IsTrue(positions.Contains(new Position(3, 4)));
        }

        [TestMethod]
        public void MapModel_CreatesMapWithCorrectSize()
        {
            var map = new MapModel(50, 50);

            Assert.AreEqual(50, map.Width);
            Assert.AreEqual(50, map.Height);
        }

        [TestMethod]
        public void MapModel_PositionInsideMap_IsInBounds()
        {
            var map = new MapModel(50, 50);

            var position = new Position(25, 25);

            Assert.IsTrue(map.IsPositionInBounds(position));
        }

        [TestMethod]
        public void MapModel_AreaInsideMap_IsInBounds()
        {
            var map = new MapModel(50, 50);

            var placement = new Placement(
                new Position(10, 10),
                new Area(5, 5));

            Assert.IsTrue(map.IsAreaInBounds(placement));
        }

        [TestMethod]
        public void MapModel_CanStoreTile()
        {
            var map = new MapModel(10, 10);

            var tile = new TileModel
            {
                Position = new Position(3, 4),
                Terrain = Domain.Common.Enums.TerrainType.Plain
            };

            map[3, 4] = tile;

            Assert.AreEqual(tile, map[3, 4]);
        }

        [TestMethod]
        public void TileModel_EmptyPlainTile_CanPlaceObject()
        {
            var tile = new TileModel
            {
                Position = new Position(3, 4),
                Terrain = Domain.Common.Enums.TerrainType.Plain
            };

            var airport = new Airport(new Area(2, 2));

            Assert.IsTrue(tile.CanPlace(airport));
        }

        [TestMethod]
        public void Airport_HasCorrectArea()
        {
            var airport = new Airport(new Area(2, 6));

            Assert.AreEqual(2, airport.Area.Width);
            Assert.AreEqual(6, airport.Area.Height);
        }

        [TestMethod]
        public void Airport_IsBuilding()
        {
            var airport = new Airport(new Area(2, 6));

            Assert.IsInstanceOfType(airport, typeof(Building));
        }

        [TestMethod]
        public void Seaport_HasCorrectArea()
        {
            var seaport = new Seaport(new Area(2, 6));

            Assert.AreEqual(2, seaport.Area.Width);
            Assert.AreEqual(6, seaport.Area.Height);
        }

        [TestMethod]
        public void Plane_IsMovingEntity()
        {
            var plane = new Plane(5.0f);

            Assert.IsInstanceOfType(plane, typeof(MovingEntity));
        }

        [TestMethod]
        public void Ship_IsMovingEntity()
        {
            var ship = new Ship(3.0f);

            Assert.IsInstanceOfType(ship, typeof(MovingEntity));
        }
    }
}