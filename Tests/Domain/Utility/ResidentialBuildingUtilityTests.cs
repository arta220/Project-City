using Domain.Buildings;
using Domain.Enums;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class ResidentialBuildingUtilityTests
    {
        [TestMethod]
        public void ResidentialBuilding_HasUtilityManager()
        {
            // Arrange & Act
            var building = new ResidentialBuilding(floors: 5, maxOccupancy: 100, area: new Area(3, 3));

            // Assert
            Assert.IsNotNull(building.Utilities);
            Assert.IsInstanceOfType(building.Utilities, typeof(UtilityManager));
        }

        [TestMethod]
        public void ResidentialBuilding_UtilitiesInitiallyWorking()
        {
            // Arrange & Act
            var building = new ResidentialBuilding(floors: 3, maxOccupancy: 50, area: new Area(2, 2));

            // Assert
            Assert.IsFalse(building.Utilities.HasBrokenUtilities);
            Assert.IsTrue(building.Utilities.IsUtilityWorking(UtilityType.Electricity));
        }

        [TestMethod]
        public void ResidentialBuilding_BreakUtility_AffectsBuilding()
        {
            // Arrange
            var building = new ResidentialBuilding(floors: 2, maxOccupancy: 20, area: new Area(1, 1));

            // Act
            building.Utilities.BreakUtility(UtilityType.Gas);

            // Assert
            Assert.IsTrue(building.Utilities.HasBrokenUtilities);
            Assert.IsFalse(building.Utilities.IsUtilityWorking(UtilityType.Gas));
        }

        [TestMethod]
        public void ResidentialBuilding_FixUtility_RestoresService()
        {
            // Arrange
            var building = new ResidentialBuilding(floors: 4, maxOccupancy: 80, area: new Area(3, 2));
            building.Utilities.BreakUtility(UtilityType.Water);

            // Act
            building.Utilities.FixUtility(UtilityType.Water);

            // Assert
            Assert.IsFalse(building.Utilities.HasBrokenUtilities);
            Assert.IsTrue(building.Utilities.IsUtilityWorking(UtilityType.Water));
        }
    }
}