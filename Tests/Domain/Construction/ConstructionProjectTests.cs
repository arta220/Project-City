using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Buildings.Construction;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using System.Collections.Generic;

namespace Tests.Domain.Construction
{
    [TestClass]
    public class ConstructionProjectTests
    {
        [TestMethod]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            var targetFactory = new SmallHouseFactory();
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 },
                { ConstructionMaterialType.Cement, 50 }
            };

            // Act
            var project = new ConstructionProject(
                targetFactory,
                requiredMaterials,
                constructionSpeed: 2,
                minWorkersRequired: 3,
                totalTicksRequired: 150
            );

            // Assert
            Assert.AreEqual(targetFactory, project.TargetBuildingFactory);
            Assert.AreEqual(requiredMaterials, project.RequiredMaterials);
            Assert.AreEqual(2, project.ConstructionSpeed);
            Assert.AreEqual(3, project.MinWorkersRequired);
            Assert.AreEqual(150, project.TotalTicksRequired);
            Assert.AreEqual(0, project.Progress);
            Assert.AreEqual(0, project.CurrentTicks);
        }

        [TestMethod]
        public void HasAllMaterials_ReturnsTrue_WhenAllMaterialsAvailable()
        {
            // Arrange
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 },
                { ConstructionMaterialType.Cement, 50 }
            };
            var project = new ConstructionProject(new SmallHouseFactory(), requiredMaterials);

            var availableMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 150 },
                { ConstructionMaterialType.Cement, 75 },
                { ConstructionMaterialType.Sand, 20 }
            };

            // Act
            var result = project.HasAllMaterials(availableMaterials);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasAllMaterials_ReturnsFalse_WhenMaterialsInsufficient()
        {
            // Arrange
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 },
                { ConstructionMaterialType.Cement, 50 }
            };
            var project = new ConstructionProject(new SmallHouseFactory(), requiredMaterials);

            var availableMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 80 }, // Недостаточно кирпичей
                { ConstructionMaterialType.Cement, 75 }
            };

            // Act
            var result = project.HasAllMaterials(availableMaterials);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasAllMaterials_ReturnsFalse_WhenMaterialsMissing()
        {
            // Arrange
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 },
                { ConstructionMaterialType.Cement, 50 }
            };
            var project = new ConstructionProject(new SmallHouseFactory(), requiredMaterials);

            var availableMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 150 }
                // Cement отсутствует
            };

            // Act
            var result = project.HasAllMaterials(availableMaterials);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetMaterialsReadiness_ReturnsOne_WhenAllMaterialsAvailable()
        {
            // Arrange
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 },
                { ConstructionMaterialType.Cement, 50 }
            };
            var project = new ConstructionProject(new SmallHouseFactory(), requiredMaterials);

            var availableMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 },
                { ConstructionMaterialType.Cement, 50 }
            };

            // Act
            var readiness = project.GetMaterialsReadiness(availableMaterials);

            // Assert
            Assert.AreEqual(1.0, readiness, 0.001);
        }

        [TestMethod]
        public void GetMaterialsReadiness_ReturnsHalf_WhenHalfMaterialsAvailable()
        {
            // Arrange
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 },
                { ConstructionMaterialType.Cement, 50 }
            };
            var project = new ConstructionProject(new SmallHouseFactory(), requiredMaterials);

            var availableMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 50 },
                { ConstructionMaterialType.Cement, 25 }
            };

            // Act
            var readiness = project.GetMaterialsReadiness(availableMaterials);

            // Assert
            Assert.AreEqual(0.5, readiness, 0.001);
        }

        [TestMethod]
        public void GetMaterialsReadiness_ReturnsZero_WhenNoMaterialsAvailable()
        {
            // Arrange
            var requiredMaterials = new Dictionary<Enum, int>
            {
                { ConstructionMaterialType.Bricks, 100 }
            };
            var project = new ConstructionProject(new SmallHouseFactory(), requiredMaterials);

            var availableMaterials = new Dictionary<Enum, int>();

            // Act
            var readiness = project.GetMaterialsReadiness(availableMaterials);

            // Assert
            Assert.AreEqual(0.0, readiness, 0.001);
        }

        [TestMethod]
        public void GetMaterialsReadiness_ReturnsZero_WhenNoMaterialsRequired()
        {
            // Arrange
            var requiredMaterials = new Dictionary<Enum, int>();
            var project = new ConstructionProject(new SmallHouseFactory(), requiredMaterials);

            var availableMaterials = new Dictionary<Enum, int>();

            // Act
            var readiness = project.GetMaterialsReadiness(availableMaterials);

            // Assert
            Assert.AreEqual(1.0, readiness, 0.001);
        }
    }
}
