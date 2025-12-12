/*using Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class ParkTypeTests
    {
        [TestMethod]
        public void ParkType_HasCorrectValues()
        {
            // Arrange & Act
            var urbanPark = ParkType.UrbanPark;
            var botanicalGarden = ParkType.BotanicalGarden;
            var playground = ParkType.Playground;
            var square = ParkType.Square;
            var recreationArea = ParkType.RecreationArea;

            // Assert
            Assert.AreEqual(0, (int)urbanPark);
            Assert.AreEqual(1, (int)botanicalGarden);
            Assert.AreEqual(2, (int)playground);
            Assert.AreEqual(3, (int)square);
            Assert.AreEqual(4, (int)recreationArea);
        }

        [TestMethod]
        public void ParkType_AllValuesDefined()
        {
            // Arrange & Act
            var values = Enum.GetValues<ParkType>();

            // Assert
            Assert.AreEqual(5, values.Length);
            Assert.IsTrue(values.Contains(ParkType.UrbanPark));
            Assert.IsTrue(values.Contains(ParkType.BotanicalGarden));
            Assert.IsTrue(values.Contains(ParkType.Playground));
            Assert.IsTrue(values.Contains(ParkType.Square));
            Assert.IsTrue(values.Contains(ParkType.RecreationArea));
        }
    }
}
*/