using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Construction;
using Domain.Buildings.Construction;
using Domain.Common.Enums;
using Domain.Common.Time;
using Tests.Mocks;
using System.Collections.Generic;

namespace Tests.Domain.Construction
{
    [TestClass]
    public class ConstructionServiceTests
    {
        private ConstructionService _service = null!;
        private FakeBuildingRegistry _buildingRegistry = null!;
        private ConstructionSite _testSite = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _buildingRegistry = new FakeBuildingRegistry();

            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 },
                { ConstructionMaterialType.Cement, 50 }
            };
            var project = new ConstructionProject(
                new Domain.Factories.SmallHouseFactory(),
                requiredMaterials,
                constructionSpeed: 2,
                minWorkersRequired: 1,
                totalTicksRequired: 50
            );

            _testSite = new ConstructionSite(new Domain.Map.Area(3, 3), project);
            _service = new ConstructionService(_buildingRegistry);
        }

        [TestMethod]
        public void StartConstruction_ReturnsTrue_WhenConstructionStartedSuccessfully()
        {
            // Act
            var result = _service.StartConstruction(_testSite);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(ConstructionSiteStatus.Preparing, _testSite.Status);
            Assert.IsFalse(_testSite.IsCancelled);
        }

        [TestMethod]
        public void StartConstruction_ReturnsFalse_WhenConstructionAlreadyStarted()
        {
            // Arrange
            _service.StartConstruction(_testSite);

            // Act
            var result = _service.StartConstruction(_testSite);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StartConstruction_ReturnsFalse_WhenSiteIsNull()
        {
            // Act
            var result = _service.StartConstruction(null!);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StartConstruction_ReturnsFalse_WhenProjectIsNull()
        {
            // Arrange
            var siteWithoutProject = new ConstructionSite(new Domain.Map.Area(3, 3), null!);

            // Act
            var result = _service.StartConstruction(siteWithoutProject);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CancelConstruction_ReturnsTrue_WhenConstructionCancelledSuccessfully()
        {
            // Arrange
            _service.StartConstruction(_testSite);

            // Act
            var result = _service.CancelConstruction(_testSite);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_testSite.IsCancelled);
            Assert.AreEqual(ConstructionSiteStatus.Cancelled, _testSite.Status);
        }

        [TestMethod]
        public void CancelConstruction_ReturnsFalse_WhenConstructionNotStarted()
        {
            // Act
            var result = _service.CancelConstruction(_testSite);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CancelConstruction_ReturnsFalse_WhenSiteIsNull()
        {
            // Act
            var result = _service.CancelConstruction(null!);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetActiveConstructionSites_ReturnsEmptyCollection_WhenNoActiveSites()
        {
            // Act
            var sites = _service.GetActiveConstructionSites();

            // Assert
            Assert.AreEqual(0, new List<ConstructionSite>(sites).Count);
        }

        [TestMethod]
        public void GetActiveConstructionSites_ReturnsActiveSites()
        {
            // Arrange
            _service.StartConstruction(_testSite);

            // Act
            var sites = _service.GetActiveConstructionSites();

            // Assert
            var sitesList = new List<ConstructionSite>(sites);
            Assert.AreEqual(1, sitesList.Count);
            Assert.AreEqual(_testSite, sitesList[0]);
        }

        [TestMethod]
        public void Update_ProcessesConstruction_WhenMaterialsAndWorkersAvailable()
        {
            // Arrange
            _service.StartConstruction(_testSite);

            // Добавим материалы и имитируем рабочих
            _testSite.AddMaterials(ConstructionMaterialType.Bricks, 100);
            _testSite.AddMaterials(ConstructionMaterialType.Cement, 50);
            // В реальности рабочие добавляются через систему граждан

            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            Assert.AreEqual(ConstructionSiteStatus.Building, _testSite.Status);
            Assert.AreEqual(2, _testSite.Project.CurrentTicks); // ConstructionSpeed = 2
        }

        [TestMethod]
        public void Update_StaysInPreparing_WhenNoMaterials()
        {
            // Arrange
            _service.StartConstruction(_testSite);
            // Не добавляем материалы

            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            Assert.AreEqual(ConstructionSiteStatus.Preparing, _testSite.Status);
            Assert.AreEqual(0, _testSite.Project.CurrentTicks);
        }

        [TestMethod]
        public void Update_CompletesConstruction_WhenProgressReaches100()
        {
            // Arrange
            _service.StartConstruction(_testSite);
            _testSite.AddMaterials(ConstructionMaterialType.Bricks, 100);
            _testSite.AddMaterials(ConstructionMaterialType.Cement, 50);

            // Имитируем завершение строительства
            _testSite.Project.CurrentTicks = 48; // Еще 2 тика до завершения
            _testSite.Status = ConstructionSiteStatus.Building;

            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            Assert.AreEqual(ConstructionSiteStatus.Completed, _testSite.Status);
            Assert.AreEqual(100, _testSite.Project.Progress);
            // Материалы должны быть потреблены
            Assert.IsFalse(_testSite.MaterialsBank.ContainsKey(ConstructionMaterialType.Bricks));
            Assert.IsFalse(_testSite.MaterialsBank.ContainsKey(ConstructionMaterialType.Cement));
        }

        [TestMethod]
        public void Update_RemovesCancelledSites()
        {
            // Arrange
            _service.StartConstruction(_testSite);
            _service.CancelConstruction(_testSite);

            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            var sites = _service.GetActiveConstructionSites();
            Assert.AreEqual(0, new List<ConstructionSite>(sites).Count);
        }

        [TestMethod]
        public void Update_ProgressesConstructionCorrectly()
        {
            // Arrange
            _service.StartConstruction(_testSite);
            _testSite.AddMaterials(ConstructionMaterialType.Bricks, 100);
            _testSite.AddMaterials(ConstructionMaterialType.Cement, 50);
            _testSite.Status = ConstructionSiteStatus.Building;

            var time = new SimulationTime(1);

            // Act
            _service.Update(time);

            // Assert
            Assert.AreEqual(4, _testSite.Project.CurrentTicks); // 0 + 2 + 2 (два обновления)
            Assert.AreEqual(8, _testSite.Project.Progress); // (4 / 50) * 100 = 8%
        }
    }
}
