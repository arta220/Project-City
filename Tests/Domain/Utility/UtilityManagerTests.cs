//using Domain.Buildings;
//using Domain.Enums;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Tests.Domain
//{
//    [TestClass]
//    public class UtilityManagerTests
//    {
//        [TestMethod]
//        public void UtilityManager_Initialization_AllUtilitiesWorking()
//        {
//            // Arrange & Act
//            var manager = new UtilityManager();

//            // Assert
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Electricity));
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Water));
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Gas));
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Waste));
//            Assert.IsFalse(manager.HasBrokenUtilities);
//        }

//        [TestMethod]
//        public void BreakUtility_SetsUtilityToBroken()
//        {
//            // Arrange
//            var manager = new UtilityManager();

//            // Act
//            manager.BreakUtility(UtilityType.Electricity);

//            // Assert
//            Assert.IsFalse(manager.IsUtilityWorking(UtilityType.Electricity));
//            Assert.IsTrue(manager.HasBrokenUtilities);
//        }

//        [TestMethod]
//        public void FixUtility_RepairsBrokenUtility()
//        {
//            // Arrange
//            var manager = new UtilityManager();
//            manager.BreakUtility(UtilityType.Water);

//            // Act
//            manager.FixUtility(UtilityType.Water);

//            // Assert
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Water));
//            Assert.IsFalse(manager.HasBrokenUtilities);
//        }

//        [TestMethod]
//        public void BreakUtility_AlreadyBroken_NoChange()
//        {
//            // Arrange
//            var manager = new UtilityManager();
//            manager.BreakUtility(UtilityType.Gas);
//            var initialStates = manager.States.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

//            // Act
//            manager.BreakUtility(UtilityType.Gas); // Повторная поломка

//            // Assert
//            Assert.IsFalse(manager.IsUtilityWorking(UtilityType.Gas));
//            CollectionAssert.AreEqual(initialStates, manager.States.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
//        }

//        [TestMethod]
//        public void FixUtility_AlreadyWorking_NoChange()
//        {
//            // Arrange
//            var manager = new UtilityManager();
//            var initialStates = manager.States.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

//            // Act
//            manager.FixUtility(UtilityType.Electricity); // Попытка починить работающую

//            // Assert
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Electricity));
//            CollectionAssert.AreEqual(initialStates, manager.States.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
//        }

//        [TestMethod]
//        public void HasBrokenUtilities_WhenAllUtilitiesWorking_ReturnsFalse()
//        {
//            // Arrange
//            var manager = new UtilityManager();

//            // Act & Assert
//            Assert.IsFalse(manager.HasBrokenUtilities);
//        }

//        [TestMethod]
//        public void HasBrokenUtilities_WhenOneUtilityBroken_ReturnsTrue()
//        {
//            // Arrange
//            var manager = new UtilityManager();

//            // Act
//            manager.BreakUtility(UtilityType.Waste);

//            // Assert
//            Assert.IsTrue(manager.HasBrokenUtilities);
//        }

//        [TestMethod]
//        public void States_ReturnsCorrectDictionary()
//        {
//            // Arrange
//            var manager = new UtilityManager();

//            // Act
//            var states = manager.States;

//            // Assert
//            Assert.AreEqual(4, states.Count);
//            Assert.IsTrue(states.ContainsKey(UtilityType.Electricity));
//            Assert.IsTrue(states.ContainsKey(UtilityType.Water));
//            Assert.IsTrue(states.ContainsKey(UtilityType.Gas));
//            Assert.IsTrue(states.ContainsKey(UtilityType.Waste));
//        }

//        [TestMethod]
//        public void MultipleUtilitiesBreakAndFix_Scenario()
//        {
//            // Arrange
//            var manager = new UtilityManager();

//            // Act - ломаем несколько систем
//            manager.BreakUtility(UtilityType.Electricity);
//            manager.BreakUtility(UtilityType.Water);

//            // Assert - проверяем что обе сломаны
//            Assert.IsFalse(manager.IsUtilityWorking(UtilityType.Electricity));
//            Assert.IsFalse(manager.IsUtilityWorking(UtilityType.Water));
//            Assert.IsTrue(manager.HasBrokenUtilities);

//            // Act - чиним одну
//            manager.FixUtility(UtilityType.Electricity);

//            // Assert - одна починена, одна еще сломана
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Electricity));
//            Assert.IsFalse(manager.IsUtilityWorking(UtilityType.Water));
//            Assert.IsTrue(manager.HasBrokenUtilities);

//            // Act - чиним вторую
//            manager.FixUtility(UtilityType.Water);

//            // Assert - все починено
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Electricity));
//            Assert.IsTrue(manager.IsUtilityWorking(UtilityType.Water));
//            Assert.IsFalse(manager.HasBrokenUtilities);
//        }
//    }
//}