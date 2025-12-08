using Domain.Buildings.Residential;
using Domain.Common.Enums;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain
{
    [TestClass]
    public class UtilityIntegrationTests
    {
        [TestMethod]
        public void MultipleBuildings_IndependentUtilityStates()
        {
            // Arrange
            var building1 = new ResidentialBuilding(2, 40, new Area(2, 2));
            var building2 = new ResidentialBuilding(3, 60, new Area(3, 2));

            // Act
            building1.Utilities.BreakUtility(UtilityType.Electricity);

            // Assert
            Assert.IsTrue(building1.Utilities.HasBrokenUtilities);
            Assert.IsFalse(building2.Utilities.HasBrokenUtilities);
            Assert.IsFalse(building1.Utilities.IsUtilityWorking(UtilityType.Electricity));
            Assert.IsTrue(building2.Utilities.IsUtilityWorking(UtilityType.Electricity));
        }

        [TestMethod]
        public void CompleteUtilityFailureAndRestoration_Scenario()
        {
            // Arrange
            var building = new ResidentialBuilding(5, 100, new Area(4, 4));

            // Act - ломаем все системы
            building.Utilities.BreakUtility(UtilityType.Electricity);
            building.Utilities.BreakUtility(UtilityType.Water);
            building.Utilities.BreakUtility(UtilityType.Gas);
            building.Utilities.BreakUtility(UtilityType.Waste);

            // Assert - все сломано
            Assert.IsTrue(building.Utilities.HasBrokenUtilities);
            foreach (var utility in System.Enum.GetValues<UtilityType>())
            {
                Assert.IsFalse(building.Utilities.IsUtilityWorking(utility));
            }

            // Act - чиним все системы
            foreach (var utility in System.Enum.GetValues<UtilityType>())
            {
                building.Utilities.FixUtility(utility);
            }

            // Assert - все работает
            Assert.IsFalse(building.Utilities.HasBrokenUtilities);
            foreach (var utility in System.Enum.GetValues<UtilityType>())
            {
                Assert.IsTrue(building.Utilities.IsUtilityWorking(utility));
            }
        }
    }
}