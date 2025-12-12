/*using Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class UtilityTypeTests
    {
        [TestMethod]
        public void UtilityType_HasCorrectValues()
        {
            // Arrange & Act
            var electricity = UtilityType.Electricity;
            var water = UtilityType.Water;
            var gas = UtilityType.Gas;
            var waste = UtilityType.Waste;

            // Assert
            Assert.AreEqual(0, (int)electricity);
            Assert.AreEqual(1, (int)water);
            Assert.AreEqual(2, (int)gas);
            Assert.AreEqual(3, (int)waste);
        }

        [TestMethod]
        public void UtilityType_AllValuesDefined()
        {
            // Arrange & Act
            var values = Enum.GetValues<UtilityType>();

            // Assert
            Assert.AreEqual(4, values.Length);
            Assert.IsTrue(values.Contains(UtilityType.Electricity));
            Assert.IsTrue(values.Contains(UtilityType.Water));
            Assert.IsTrue(values.Contains(UtilityType.Gas));
            Assert.IsTrue(values.Contains(UtilityType.Waste));
        }

        [TestMethod]
        public void UtilityType_CanBeUsedInDictionary()
        {
            // Arrange & Act
            var utilityDict = new System.Collections.Generic.Dictionary<UtilityType, string>
            {
                [UtilityType.Electricity] = "Электричество",
                [UtilityType.Water] = "Водоснабжение",
                [UtilityType.Gas] = "Газоснабжение",
                [UtilityType.Waste] = "Вывоз отходов"
            };

            // Assert
            Assert.AreEqual(4, utilityDict.Count);
            Assert.AreEqual("Электричество", utilityDict[UtilityType.Electricity]);
        }
    }
}
*/