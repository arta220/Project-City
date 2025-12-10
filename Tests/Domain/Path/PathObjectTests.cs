using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class PathObjectTests
    {
        [TestMethod]
        public void PedestrianPath_IsMapObject_WithCorrectAreaAndType()
        {
            var path = new PedestrianPath();

            Assert.IsInstanceOfType(path, typeof(MapObject));
            Assert.AreEqual(1, path.Area.Width);
            Assert.AreEqual(1, path.Area.Height);
            Assert.AreEqual(PathType.Pedestrian, path.Type);
        }

        [TestMethod]
        public void BicyclePath_IsMapObject_WithCorrectAreaAndType()
        {
            var path = new BicyclePath();

            Assert.IsInstanceOfType(path, typeof(MapObject));
            Assert.AreEqual(1, path.Area.Width);
            Assert.AreEqual(1, path.Area.Height);
            Assert.AreEqual(PathType.Bicycle, path.Type);
        }
    }
}

