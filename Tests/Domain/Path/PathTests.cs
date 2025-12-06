using Domain.Base;
using Domain.Enums;
using Domain.Infrastructure;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class PathTests
    {
        [TestMethod]
        public void PedestrianPath_Creation_SetsCorrectType()
        {
            // Arrange & Act
            var path = new PedestrianPath();

            // Assert
            Assert.AreEqual(PathType.Pedestrian, path.Type);
            Assert.AreEqual(1, path.Area.Width);
            Assert.AreEqual(1, path.Area.Height);
        }

        [TestMethod]
        public void BicyclePath_Creation_SetsCorrectType()
        {
            // Arrange & Act
            var path = new BicyclePath();

            // Assert
            Assert.AreEqual(PathType.Bicycle, path.Type);
            Assert.AreEqual(1, path.Area.Width);
            Assert.AreEqual(1, path.Area.Height);
        }

        [TestMethod]
        public void Path_IsMapObject()
        {
            // Arrange & Act
            var pedestrianPath = new PedestrianPath();
            var bicyclePath = new BicyclePath();

            // Assert
            Assert.IsInstanceOfType(pedestrianPath, typeof(MapObject));
            Assert.IsInstanceOfType(bicyclePath, typeof(MapObject));
        }
    }
}