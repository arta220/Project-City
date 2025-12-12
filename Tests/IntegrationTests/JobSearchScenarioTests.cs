// Tests/Services/Citizens/Scenarios/JobSearchScenarioTests.cs
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.BuildingRegistry;
using Services.Citizens.Scenarios;
using Services.Citizens.Tasks;
using Services.EntityMovement.Service;
using Services.Interfaces;
using Services.Time;
using System.Collections.Generic;
using System.Linq;

namespace Tests.IntegrationTests
{
    [TestClass]
    public class JobSearchScenarioTests
    {
        [TestMethod]
        public void CanRun_ShouldReturnTrue_WhenUnemployedAdultDuringWorkingHours()
        {
            // Arrange
            var citizen = CreateCitizen(age: 25, hasJob: false);
            var timeServiceMock = new Mock<ISimulationTimeService>();
            timeServiceMock.Setup(t => t.IsWorkTime()).Returns(true);

            var scenario = CreateScenario(timeServiceMock);

            // Act
            var result = scenario.CanRun(citizen, timeServiceMock.Object);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanRun_ShouldReturnFalse_WhenAlreadyHasJob()
        {
            // Arrange
            var citizen = CreateCitizen(age: 30, hasJob: true);
            var timeServiceMock = new Mock<ISimulationTimeService>();

            var scenario = CreateScenario(timeServiceMock);

            // Act
            var result = scenario.CanRun(citizen, timeServiceMock.Object);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BuildTasks_ShouldCreateTasks_WhenJobsAvailable()
        {
            // Arrange
            var citizen = CreateCitizen(age: 25, hasJob: false);
            var building = new Mock<Building>(1, 10, new Area(2, 2)).Object;

            var findJobServiceMock = new Mock<IFindJobService>();
            findJobServiceMock.Setup(s => s.FindJob(citizen.Profession))
                .Returns(new List<Building> { building });

            var scenario = CreateScenario(findJobService: findJobServiceMock);

            // Act
            scenario.BuildTasks(citizen);

            // Assert
            Assert.AreEqual(CitizenState.LookingForJob, citizen.State);
            Assert.AreEqual(2, citizen.Tasks.Count);
        }

        [TestMethod]
        public void BuildTasks_ShouldSetIdleState_WhenNoJobs()
        {
            // Arrange
            var citizen = CreateCitizen(age: 25, hasJob: false);
            citizen.State = CitizenState.LookingForJob;

            var findJobServiceMock = new Mock<IFindJobService>();
            findJobServiceMock.Setup(s => s.FindJob(citizen.Profession))
                .Returns(new List<Building>());

            var scenario = CreateScenario(findJobService: findJobServiceMock);

            // Act
            scenario.BuildTasks(citizen);

            // Assert
            Assert.AreEqual(CitizenState.Idle, citizen.State);
            Assert.AreEqual(0, citizen.Tasks.Count);
        }

        private JobSearchScenario CreateScenario(
            Mock<ISimulationTimeService>? timeServiceMock = null,
            Mock<IFindJobService>? findJobService = null)
        {
            timeServiceMock ??= new Mock<ISimulationTimeService>();
            findJobService ??= new Mock<IFindJobService>();

            return new JobSearchScenario(
                findJobService.Object,
                new Mock<IEntityMovementService>().Object,
                new Mock<IBuildingRegistry>().Object,
                timeServiceMock.Object);
        }

        private Citizen CreateCitizen(int age, bool hasJob)
        {
            return new Citizen(new Area(1, 1), speed: 1.0f)
            {
                Age = age,
                Profession = CitizenProfession.Chef,
                WorkPlace = hasJob ? new Mock<Building>(1, 10, new Area(2, 2)).Object : null
            };
        }
    }
}