using System.Windows;
using CitySimulatorWPF.Converters;
using Domain.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.CitySimulatorWPF
{


    [TestClass]
    public class MapUiTests
    {
        [TestMethod]
        public void TerrainTypeToStringRuConverter_ReturnsRussianNames()
        {
            // Arrange
            var converter = new TerrainTypeToStringRuConverter();

            // Act & Assert
            Assert.AreEqual("Вода", converter.Convert(TerrainType.Water, null, null, null));
            Assert.AreEqual("Равнина", converter.Convert(TerrainType.Plain, null, null, null));
            Assert.AreEqual("Луг", converter.Convert(TerrainType.Meadow, null, null, null));
            Assert.AreEqual("Лес", converter.Convert(TerrainType.Forest, null, null, null));
            Assert.AreEqual("Горы", converter.Convert(TerrainType.Mountain, null, null, null));
        }

        [TestMethod]
        public void ResourceTypeToStringRuConverter_ReturnsRussianNames()
        {
            // Arrange
            var converter = new ResourceTypeToStringRuConverter();

            // Act & Assert
            Assert.AreEqual("Нет", converter.Convert(NaturalResourceType.None, null, null, null));
            Assert.AreEqual("Железо", converter.Convert(NaturalResourceType.Iron, null, null, null));
            Assert.AreEqual("Медь", converter.Convert(NaturalResourceType.Copper, null, null, null));
            Assert.AreEqual("Нефть", converter.Convert(NaturalResourceType.Oil, null, null, null));
            Assert.AreEqual("Газ", converter.Convert(NaturalResourceType.Gas, null, null, null));
            Assert.AreEqual("Дерево", converter.Convert(NaturalResourceType.Wood, null, null, null));
        }

        [TestMethod]
        public void ResourceVisibilityConverter_Collapsed_WhenNoResource()
        {
            // Arrange
            var converter = new ResourceVisibilityConverter();

            // Act
            var result = converter.Convert(
                new object[] { false, true }, // HasResource = false, ShowResources = true
                typeof(Visibility),
                null,
                null);

            // Assert
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void ResourceVisibilityConverter_Collapsed_WhenShowResourcesFalse()
        {
            // Arrange
            var converter = new ResourceVisibilityConverter();

            // Act
            var result = converter.Convert(
                new object[] { true, false }, // HasResource = true, ShowResources = false
                typeof(Visibility),
                null,
                null);

            // Assert
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void ResourceVisibilityConverter_Visible_WhenHasResourceAndShowResourcesTrue()
        {
            // Arrange
            var converter = new ResourceVisibilityConverter();

            // Act
            var result = converter.Convert(
                new object[] { true, true }, // HasResource = true, ShowResources = true
                typeof(Visibility),
                null,
                null);

            // Assert
            Assert.AreEqual(Visibility.Visible, result);
        }
    }
}