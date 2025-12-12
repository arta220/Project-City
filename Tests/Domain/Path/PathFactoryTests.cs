/*using Domain.Factories;
using Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class PathFactoryTests
    {
        [TestMethod]
        public void PedestrianPathFactory_CreatesCorrectType()
        {
            // Arrange
            var factory = new PedestrianPathFactory();

            // Act
            var path = factory.Create();

            // Assert
            Assert.IsInstanceOfType(path, typeof(PedestrianPath));
        }

        [TestMethod]
        public void BicyclePathFactory_CreatesCorrectType()
        {
            // Arrange
            var factory = new BicyclePathFactory();

            // Act
            var path = factory.Create();

            // Assert
            Assert.IsInstanceOfType(path, typeof(BicyclePath));
        }

        [TestMethod]
        public void PathFactories_CreateUniqueInstances()
        {
            // Arrange
            var pedestrianFactory = new PedestrianPathFactory();
            var bicycleFactory = new BicyclePathFactory();

            // Act
            var path1 = pedestrianFactory.Create();
            var path2 = pedestrianFactory.Create();
            var bikePath = bicycleFactory.Create();

            // Assert
            Assert.AreNotSame(path1, path2);
            Assert.AreNotSame(path1, bikePath);
            Assert.IsInstanceOfType(path1, typeof(PedestrianPath));
            Assert.IsInstanceOfType(bikePath, typeof(BicyclePath));
        }
    }
}
*/