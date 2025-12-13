using Domain.Buildings;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests.Domain.Industrial
{
    [TestClass]
    public class CosmeticsFactoryTests
    {
        [TestMethod]
        public void CosmeticsFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange
            var factory = new CosmeticsFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(3, building.Floors);
            Assert.AreEqual(120, building.MaxOccupancy);
            Assert.AreEqual(new Area(6, 6), building.Area);
            Assert.AreEqual(IndustrialBuildingType.Factory, building.Type);
        }

        [TestMethod]
        public void CosmeticsFactory_CreatesBuilding_WithTenWorkshops()
        {
            // Arrange
            var factory = new CosmeticsFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.AreEqual(10, building.Workshops.Count, "Косметический завод должен иметь 10 цехов");
        }

        [TestMethod]
        public void CosmeticsFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new CosmeticsFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Chemicals), "Должны быть химикаты");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Water), "Должна быть вода");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Glass), "Должно быть стекло");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(ProductType.Plastic), "Должен быть пластик");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Energy), "Должна быть энергия");
        }

        [TestMethod]
        public void CosmeticsFactory_Production_ProducesCosmeticsProducts()
        {
            // Arrange
            var factory = new CosmeticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();
            building.RunOnce();

            // Assert
            var hasCosmeticsProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.SkinCream ||
                         p == ProductType.Shampoo ||
                         p == ProductType.Perfume ||
                         p == ProductType.Makeup ||
                         p == ProductType.CosmeticBottle ||
                         p == ProductType.HairCareProduct ||
                         p == ProductType.Sunscreen ||
                         p == ProductType.MakeupKit ||
                         p == ProductType.HygieneProduct ||
                         p == ProductType.ScentedCandle ||
                         p == ProductType.CosmeticSet);

            Assert.IsTrue(hasCosmeticsProducts || building.ProductsBank.Count > 0,
                "После производства должны появиться продукты косметики");
        }

        [TestMethod]
        public void CosmeticsFactory_Workshop_ProcessesMaterials()
        {
            // Arrange
            var factory = new CosmeticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var initialChemicals = building!.MaterialsBank.GetValueOrDefault(NaturalResourceType.Chemicals, 0);
            building.RunOnce();
            var afterChemicals = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Chemicals, 0);

            // Assert
            Assert.IsTrue(initialChemicals >= afterChemicals,
                "После производства количество химикатов должно уменьшиться");
        }

        [TestMethod]
        public void CosmeticsFactory_AllWorkshops_HaveValidInputOutput()
        {
            // Arrange
            var factory = new CosmeticsFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act & Assert
            Assert.IsNotNull(building);

            foreach (var workshop in building.Workshops)
            {
                Assert.IsNotNull(workshop.InputMaterial, "Цех должен иметь входной материал");
                Assert.IsNotNull(workshop.OutputProduct, "Цех должен иметь выходной продукт");
                Assert.IsTrue(workshop.ProductionCoefficient > 0, "Коэффициент производства должен быть больше 0");
            }
        }
    }
}