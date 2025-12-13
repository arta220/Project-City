// Tests/Domain/Disaster/DisasterManagerTests.cs
using Domain.Buildings.Disaster;
using Domain.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain.Disaster
{
    [TestClass]
    public class DisasterManagerTests
    {
        [TestMethod]
        public void DisasterManager_InitialState_NoDisasters()
        {
            // Arrange
            var manager = new DisasterManager();

            // Act & Assert
            Assert.IsFalse(manager.HasDisaster);
            Assert.IsFalse(manager.IsDisasterActive(DisasterType.Fire));
            Assert.IsFalse(manager.IsDisasterActive(DisasterType.Flood));
            Assert.IsFalse(manager.IsDisasterActive(DisasterType.Blizzard));
        }

        [TestMethod]
        public void StartDisaster_Fire_ActivatesFire()
        {
            // Arrange
            var manager = new DisasterManager();
            int currentTick = 100;

            // Act
            manager.StartDisaster(DisasterType.Fire, currentTick);

            // Assert
            Assert.IsTrue(manager.HasDisaster);
            Assert.IsTrue(manager.IsDisasterActive(DisasterType.Fire));
            Assert.IsFalse(manager.IsDisasterActive(DisasterType.Flood));
            Assert.IsFalse(manager.IsDisasterActive(DisasterType.Blizzard));
            Assert.AreEqual(100, manager.GetStartTick(DisasterType.Fire));
        }

        [TestMethod]
        public void StartDisaster_ReplacesExistingDisaster()
        {
            // Arrange
            var manager = new DisasterManager();
            manager.StartDisaster(DisasterType.Fire, 100);

            // Act
            manager.StartDisaster(DisasterType.Flood, 200);

            // Assert
            Assert.IsTrue(manager.HasDisaster);
            Assert.IsFalse(manager.IsDisasterActive(DisasterType.Fire)); // Fire should be replaced
            Assert.IsTrue(manager.IsDisasterActive(DisasterType.Flood)); // Flood is now active
            Assert.AreEqual(200, manager.GetStartTick(DisasterType.Flood));
        }

        [TestMethod]
        public void StopDisaster_DeactivatesDisaster()
        {
            // Arrange
            var manager = new DisasterManager();
            manager.StartDisaster(DisasterType.Blizzard, 100);

            // Act
            manager.StopDisaster(DisasterType.Blizzard);

            // Assert
            Assert.IsFalse(manager.HasDisaster);
            Assert.IsFalse(manager.IsDisasterActive(DisasterType.Blizzard));
        }

        [TestMethod]
        public void States_Property_ReturnsAllStates()
        {
            // Arrange
            var manager = new DisasterManager();
            manager.StartDisaster(DisasterType.Fire, 100);

            // Act
            var states = manager.States;

            // Assert
            Assert.IsTrue(states[DisasterType.Fire]);
            Assert.IsFalse(states[DisasterType.Flood]);
            Assert.IsFalse(states[DisasterType.Blizzard]);
        }
    }
}