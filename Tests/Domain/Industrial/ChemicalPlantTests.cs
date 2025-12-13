using System;
using System.Linq;
using Domain.Buildings.Industrial;
using Domain.Citizens.States;
using Domain.Common.Enums;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Domain.Buildings.Industrial
{
    [TestClass]
    public class ChemicalPlantTests
    {
        private ChemicalPlant _chemicalPlant;
        private Area _testArea;

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void InitializeForSpecialization_Petrochemicals_ShouldSetUpCorrectVacancies()
        {
            // Arrange & Act
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);

            // Assert
            Assert.IsTrue(_chemicalPlant.Vacancies.Count > 0);
            Assert.AreEqual(8, _chemicalPlant.Vacancies[CitizenProfession.ChemicalEngineer]);
            Assert.AreEqual(6, _chemicalPlant.Vacancies[CitizenProfession.Chemist]);
            Assert.AreEqual(30, _chemicalPlant.Vacancies[CitizenProfession.FactoryWorker]);
        }

        [TestMethod]
        public void InitializeForSpecialization_AgriculturalChemicals_ShouldSetUpCorrectVacancies()
        {
            // Arrange & Act
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.AgriculturalChemicals);

            // Assert
            Assert.IsTrue(_chemicalPlant.Vacancies.Count > 0);
            Assert.AreEqual(6, _chemicalPlant.Vacancies[CitizenProfession.Chemist]);
            Assert.AreEqual(25, _chemicalPlant.Vacancies[CitizenProfession.FactoryWorker]);
        }

        [TestMethod]
        public void InitializeForSpecialization_ConsumerChemicals_ShouldSetUpCorrectVacancies()
        {
            // Arrange & Act
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.ConsumerChemicals);

            // Assert
            Assert.IsTrue(_chemicalPlant.Vacancies.Count > 0);
            Assert.AreEqual(8, _chemicalPlant.Vacancies[CitizenProfession.Chemist]);
            Assert.AreEqual(35, _chemicalPlant.Vacancies[CitizenProfession.FactoryWorker]);
        }

        [TestMethod]
        public void InitializeForSpecialization_IndustrialChemicals_ShouldSetUpCorrectVacancies()
        {
            // Arrange & Act
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.IndustrialChemicals);

            // Assert
            Assert.IsTrue(_chemicalPlant.Vacancies.Count > 0);
            Assert.AreEqual(10, _chemicalPlant.Vacancies[CitizenProfession.ChemicalEngineer]);
            Assert.AreEqual(40, _chemicalPlant.Vacancies[CitizenProfession.FactoryWorker]);
        }

        [TestMethod]
        public void InitializeForSpecialization_ShouldAddWorkshops()
        {
            // Arrange & Act
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);

            // Assert
            Assert.IsTrue(_chemicalPlant.Workshops.Count > 0);
        }

        [TestMethod]
        public void InitializeForSpecialization_ShouldInitializeMaterialsBank()
        {
            // Arrange & Act
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);

            // Assert
            Assert.IsTrue(_chemicalPlant.MaterialsBank.Count > 0);
            Assert.AreEqual(1000, _chemicalPlant.MaterialsBank[ResourceType.Water]); // Общий для всех
        }

        [TestMethod]
        public void UpgradeTechnology_ShouldIncreaseTechnologyLevelAndReducePollution()
        {
            // Arrange
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);
            var initialTechLevel = _chemicalPlant.TechnologyLevel;
            var initialPollution = _chemicalPlant.PollutionLevel;

            // Act
            _chemicalPlant.UpgradeTechnology();

            // Assert
            Assert.AreEqual(initialTechLevel + 1, _chemicalPlant.TechnologyLevel);
            Assert.AreEqual(Math.Max(0, initialPollution - 5), _chemicalPlant.PollutionLevel);
        }

        [TestMethod]
        public void ImproveSafety_ShouldIncreaseSafetyLevel()
        {
            // Arrange
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);
            var initialSafety = _chemicalPlant.SafetyLevel;

            // Act
            _chemicalPlant.ImproveSafety(15);

            // Assert
            Assert.AreEqual(Math.Min(100, initialSafety + 15), _chemicalPlant.SafetyLevel);
        }

        [TestMethod]
        public void ReducePollution_ShouldDecreasePollutionLevel()
        {
            // Arrange
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);
            var initialPollution = _chemicalPlant.PollutionLevel;

            // Act
            _chemicalPlant.ReducePollution(10);

            // Assert
            Assert.AreEqual(Math.Max(0, initialPollution - 10), _chemicalPlant.PollutionLevel);
        }

        [TestMethod]
        public void AddProductionLine_ShouldAddNewWorkshop()
        {
            // Arrange
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);
            var initialWorkshopCount = _chemicalPlant.Workshops.Count;

            // Act
            _chemicalPlant.AddProductionLine(ResourceType.Chemicals, ProductType.Plastics, 3);

            // Assert
            Assert.AreEqual(initialWorkshopCount + 1, _chemicalPlant.Workshops.Count);
        }

        [TestMethod]
        public void GetProductionEfficiency_ShouldReturnValueBetween10And100()
        {
            // Arrange
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);

            // Act
            var efficiency = _chemicalPlant.GetProductionEfficiency();

            // Assert
            Assert.IsTrue(efficiency >= 10, $"Efficiency should be >= 10, but was {efficiency}");
            Assert.IsTrue(efficiency <= 100, $"Efficiency should be <= 100, but was {efficiency}");
        }

        [TestMethod]
        public void GetProductionEfficiency_ShouldBeAffectedByTechnologyAndPollution()
        {
            // Arrange
            _chemicalPlant = new ChemicalPlant(4, 100, _testArea, ChemicalPlantSpecialization.Petrochemicals);
            var baseEfficiency = _chemicalPlant.GetProductionEfficiency();

            // Act
            _chemicalPlant.UpgradeTechnology(); // +5 к эффективности, -5 к загрязнению
            var afterUpgrade = _chemicalPlant.GetProductionEfficiency();

            // Assert - Эффективность должна увеличиться
            Assert.IsTrue(afterUpgrade >= baseEfficiency,
                $"Efficiency should increase after upgrade. Before: {baseEfficiency}, After: {afterUpgrade}");
        }

        [TestMethod]
        public void ChemicalPlantSpecialization_ShouldHaveFourValues()
        {
            // Arrange & Act
            var values = Enum.GetValues(typeof(ChemicalPlantSpecialization));

            // Assert
            Assert.AreEqual(4, values.Length);
        }

        [TestMethod]
        [DataRow(ChemicalPlantSpecialization.Petrochemicals, "Petrochemicals")]
        [DataRow(ChemicalPlantSpecialization.AgriculturalChemicals, "AgriculturalChemicals")]
        [DataRow(ChemicalPlantSpecialization.ConsumerChemicals, "ConsumerChemicals")]
        [DataRow(ChemicalPlantSpecialization.IndustrialChemicals, "IndustrialChemicals")]
        public void ChemicalPlantSpecialization_ShouldHaveExpectedNames(ChemicalPlantSpecialization specialization, string expectedName)
        {
            // Assert
            Assert.AreEqual(expectedName, specialization.ToString());
        }
    }
}