using Domain.Base;
using Domain.Enums;
using Domain.Factories;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class ParkFactoryTests
    {
        [TestMethod]
        public void UrbanParkFactory_CreatesCorrectPark()
        {
            // Arrange
            var factory = new UrbanParkFactory();

            // Act
            var park = factory.Create();

            // Assert
            Assert.IsInstanceOfType(park, typeof(Park));
            Assert.AreEqual(ParkType.UrbanPark, ((Park)park).Type);
            Assert.AreEqual(3, park.Area.Width);
            Assert.AreEqual(3, park.Area.Height);
        }

        [TestMethod]
        public void SquareParkFactory_CreatesCorrectPark()
        {
            // Arrange
            var factory = new SquareParkFactory();

            // Act
            var park = factory.Create();

            // Assert
            Assert.IsInstanceOfType(park, typeof(Park));
            Assert.AreEqual(ParkType.Square, ((Park)park).Type);
            Assert.AreEqual(2, park.Area.Width);
            Assert.AreEqual(3, park.Area.Height);
        }

        [TestMethod]
        public void BotanicalGardenParkFactory_CreatesCorrectPark()
        {
            // Arrange
            var factory = new BotanicalGardenParkFactory();

            // Act
            var park = factory.Create();

            // Assert
            Assert.IsInstanceOfType(park, typeof(Park));
            Assert.AreEqual(ParkType.BotanicalGarden, ((Park)park).Type);
            Assert.AreEqual(4, park.Area.Width);
            Assert.AreEqual(4, park.Area.Height);
        }

        [TestMethod]
        public void PlaygroundParkFactory_CreatesCorrectPark()
        {
            // Arrange
            var factory = new PlaygroundParkFactory();

            // Act
            var park = factory.Create();

            // Assert
            Assert.IsInstanceOfType(park, typeof(Park));
            Assert.AreEqual(ParkType.Playground, ((Park)park).Type);
            Assert.AreEqual(1, park.Area.Width);
            Assert.AreEqual(1, park.Area.Height);
        }

        [TestMethod]
        public void RecreationAreaParkFactory_CreatesCorrectPark()
        {
            // Arrange
            var factory = new RecreationAreaParkFactory();

            // Act
            var park = factory.Create();

            // Assert
            Assert.IsInstanceOfType(park, typeof(Park));
            Assert.AreEqual(ParkType.RecreationArea, ((Park)park).Type);
            Assert.AreEqual(2, park.Area.Width);
            Assert.AreEqual(3, park.Area.Height);
        }

        [TestMethod]
        public void ParkFactories_CreateUniqueInstances()
        {
            // Arrange
            var urbanFactory = new UrbanParkFactory();
            var gardenFactory = new BotanicalGardenParkFactory();

            // Act
            var urbanPark = urbanFactory.Create();
            var gardenPark = gardenFactory.Create();

            // Assert
            Assert.AreNotSame(urbanPark, gardenPark);
            Assert.IsInstanceOfType(urbanPark, typeof(Park));
            Assert.IsInstanceOfType(gardenPark, typeof(Park));
        }
    }
}