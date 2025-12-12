// Tests/Services/Citizens/Tasks/ApplyForJobTaskTests.cs
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.Citizens.Tasks;

namespace Tests.Job
{
    [TestClass]
    public class ApplyForJobTaskTests
    {
        [TestMethod]
        public void Execute_ShouldHireCitizen_WhenBuildingHasVacancy()
        {
            // Arrange
            var mockBuilding = new Mock<Building>(1, 10, new Area(2, 2));
            var citizen = CreateCitizen();

            mockBuilding.Setup(b => b.Hire(citizen)).Returns(true);

            var task = new ApplyForJobTask(mockBuilding.Object);
            var time = new SimulationTime(10000);

            // Act
            task.Execute(citizen, time);

            // Assert
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(CitizenState.Working, citizen.State);
            mockBuilding.Verify(b => b.Hire(citizen), Times.Once());
        }

        [TestMethod]
        public void Execute_ShouldSetIdleState_WhenHireFails()
        {
            // Arrange
            var mockBuilding = new Mock<Building>(1, 10, new Area(2, 2));
            var citizen = CreateCitizen();

            mockBuilding.Setup(b => b.Hire(citizen)).Returns(false);

            var task = new ApplyForJobTask(mockBuilding.Object);
            var time = new SimulationTime(10000);

            // Act
            task.Execute(citizen, time);

            // Assert
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(CitizenState.Idle, citizen.State);
        }

        private Citizen CreateCitizen()
        {
            return new Citizen(new Area(1, 1), speed: 1.0f)
            {
                Age = 25,
                Profession = CitizenProfession.Chef
            };
        }
    }
}