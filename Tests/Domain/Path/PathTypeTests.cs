using Domain.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class PathTypeTests
    {
        [TestMethod]
        public void PathType_HasCorrectValues()
        {
            // Arrange & Act
            var pedestrian = PathType.Pedestrian;
            var bicycle = PathType.Bicycle;

            // Assert
            Assert.AreEqual(0, (int)pedestrian);
            Assert.AreEqual(1, (int)bicycle);
        }

        [TestMethod]
        public void PathType_AllValuesDefined()
        {
            // Arrange & Act
            var values = Enum.GetValues<PathType>();

            // Assert
            Assert.AreEqual(2, values.Length);
            Assert.IsTrue(values.Contains(PathType.Pedestrian));
            Assert.IsTrue(values.Contains(PathType.Bicycle));
        }
    }
}