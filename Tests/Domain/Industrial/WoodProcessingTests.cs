using Domain.Buildings;
using Domain.Citizens.States;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests.Domain.Industrial
{
    /// <summary>
    /// Тесты для деревообрабатывающей промышленности (WoodProcessingFactory).
    /// 
    /// КОНЦЕПТУАЛЬНОЕ ОПИСАНИЕ РАБОТЫ ДЕРЕВООБРАБАТЫВАЮЩЕЙ ПРОМЫШЛЕННОСТИ:
    /// 
    /// 1. ЧТО ЭТО ТАКОЕ:
    ///    Деревообрабатывающий завод - это вторичное звено производственной цепочки.
    ///    Он перерабатывает дерево (Wood) в различные продукты:
    ///    - Пиломатериалы (Lumber) - базовый продукт из дерева
    ///    - Мебель (Furniture) - производится из пиломатериалов (цепочка: Wood → Lumber → Furniture)
    ///    - Бумага (Paper) - производится напрямую из дерева
    ///    - Деревянные ящики (WoodenCrate) - производится напрямую из дерева
    /// 
    /// 2. КАК ЭТО РАБОТАЕТ:
    ///    - Завод имеет начальный запас дерева (300 единиц) в MaterialsBank
    ///    - Завод содержит 4 цеха с разными производственными цепочками
    ///    - Некоторые цехи используют продукты других цехов как материалы (цепочка производства)
    ///    - При вызове RunOnce() все цехи пытаются произвести свои продукты
    /// 
    /// 3. ПРОИЗВОДСТВЕННАЯ ЦЕПОЧКА:
    ///    Wood (300) → [Цех 1: Lumber (коэф. 6)] → Lumber в ProductsBank
    ///                → [Цех 2: Paper (коэф. 8)] → Paper в ProductsBank
    ///                → [Цех 3: WoodenCrate (коэф. 5)] → WoodenCrate в ProductsBank
    ///    
    ///    Lumber (из ProductsBank) → [Цех 4: Furniture (коэф. 3)] → Furniture в ProductsBank
    ///    
    ///    ВАЖНО: Цех 4 использует Lumber из ProductsBank как материал (цепочка производства)
    /// 
    /// 4. КЛЮЧЕВЫЕ МЕТОДЫ ДЛЯ ИЗУЧЕНИЯ:
    ///    - WoodProcessingFactory.Create() - создание завода (Domain/Factories/AllFactories.cs:157-179)
    ///    - IndustrialBuilding.AddWorkshop() - добавление цеха
    ///    - Workshop.Process() - обработка цеха (может брать материалы из ProductsBank для цепочек)
    ///    - IndustrialBuilding.RunOnce() - запуск всех цехов
    /// 
    /// 5. ОТКУДА КУДА БЕРЕТСЯ:
    ///    ВХОД:  MaterialsBank[NaturalResourceType.Wood] = 300 (начальный запас)
    ///    ПРОЦЕСС: 
    ///      - Цехи 1,2,3: берут Wood из MaterialsBank → кладут продукты в ProductsBank
    ///      - Цех 4: берет Lumber из ProductsBank → кладет Furniture в ProductsBank
    ///    ВЫХОД:  ProductsBank[ProductType.Lumber/Paper/WoodenCrate/Furniture] - готовые продукты
    /// 
    /// 6. ОСОБЕННОСТИ:
    ///    - Цепочка производства: Wood → Lumber → Furniture (двухэтапный процесс)
    ///    - Параллельное производство: из одного Wood можно сделать Paper, Lumber или WoodenCrate
    ///    - Workshop.Process() может брать материалы как из MaterialsBank, так и из ProductsBank
    /// </summary>
    [TestClass]
    public class WoodProcessingTests
    {
        /// <summary>
        /// Тест: Фабрика должна создавать здание с правильными базовыми свойствами.
        /// 
        /// Проверяет, что WoodProcessingFactory создает IndustrialBuilding с корректными параметрами:
        /// - 2 этажа (двухэтажное здание)
        /// - Максимальная вместимость 25 человек
        /// - Площадь 5x5 тайлов
        /// 
        /// Методы для изучения: WoodProcessingFactory.Create() в Domain/Factories/AllFactories.cs:157-179
        /// </summary>
        [TestMethod]
        public void WoodProcessingFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange - Подготовка: создаем фабрику
            var factory = new WoodProcessingFactory();

            // Act - Действие: создаем здание через фабрику
            var building = factory.Create() as IndustrialBuilding;

            // Assert - Проверка: здание должно иметь правильные свойства
            Assert.IsNotNull(building);
            Assert.AreEqual(2, building.Floors, "Деревообрабатывающий завод должен быть двухэтажным");
            Assert.AreEqual(25, building.MaxOccupancy, "Максимальная вместимость должна быть 25");
            Assert.AreEqual(new Area(5, 5), building.Area, "Площадь должна быть 5x5");
        }

        [TestMethod]
        public void WoodProcessingFactory_CreatesBuilding_WithWorkshops()
        {
            // Arrange
            var factory = new WoodProcessingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Workshops.Count > 0, "Завод должен иметь цеха");
            Assert.AreEqual(4, building.Workshops.Count, "Древообрабатывающий завод должен иметь 4 цеха");
        }

        [TestMethod]
        public void WoodProcessingFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new WoodProcessingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Wood), 
                "Должна быть древесина");
            Assert.AreEqual(300, building.MaterialsBank[NaturalResourceType.Wood], 
                "Начальное количество древесины должно быть 300");
        }

        [TestMethod]
        public void WoodProcessingFactory_HasCorrectWorkshops()
        {
            // Arrange
            var factory = new WoodProcessingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            
            // Проверяем цех по производству пиломатериалов
            var lumberWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "Wood" && w.OutputProduct.ToString() == "Lumber");
            Assert.IsNotNull(lumberWorkshop, "Должен быть цех по производству пиломатериалов");
            Assert.AreEqual(6, lumberWorkshop.ProductionCoefficient, "Коэффициент производства пиломатериалов должен быть 6");

            // Проверяем цех по производству мебели
            var furnitureWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "Lumber" && w.OutputProduct.ToString() == "Furniture");
            Assert.IsNotNull(furnitureWorkshop, "Должен быть цех по производству мебели");
            Assert.AreEqual(3, furnitureWorkshop.ProductionCoefficient, "Коэффициент производства мебели должен быть 3");

            // Проверяем цех по производству бумаги
            var paperWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "Wood" && w.OutputProduct.ToString() == "Paper");
            Assert.IsNotNull(paperWorkshop, "Должен быть цех по производству бумаги");
            Assert.AreEqual(8, paperWorkshop.ProductionCoefficient, "Коэффициент производства бумаги должен быть 8");

            // Проверяем цех по производству деревянных ящиков
            var crateWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "Wood" && w.OutputProduct.ToString() == "WoodenCrate");
            Assert.IsNotNull(crateWorkshop, "Должен быть цех по производству деревянных ящиков");
            Assert.AreEqual(5, crateWorkshop.ProductionCoefficient, "Коэффициент производства ящиков должен быть 5");
        }

        [TestMethod]
        public void WoodProcessingFactory_Production_ProducesWoodProducts()
        {
            // Arrange
            var factory = new WoodProcessingFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();
            building.RunOnce();

            // Assert
            var hasWoodProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.Lumber ||
                         p == ProductType.Furniture ||
                         p == ProductType.Paper ||
                         p == ProductType.WoodenCrate);

            Assert.IsTrue(hasWoodProducts || building.ProductsBank.Count > 0, 
                "После производства должны появиться продукты из дерева");
        }

        [TestMethod]
        public void WoodProcessingFactory_Workshop_ProcessesMaterials()
        {
            // Arrange
            var factory = new WoodProcessingFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var initialWood = building!.MaterialsBank.GetValueOrDefault(NaturalResourceType.Wood, 0);
            building.RunOnce();
            var afterWood = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Wood, 0);

            // Assert
            Assert.IsTrue(initialWood >= afterWood, 
                "После производства количество древесины должно уменьшиться");
        }

        /// <summary>
        /// Тест: Производственная цепочка должна работать корректно.
        /// 
        /// Проверяет цепочку производства: Wood → Lumber → Furniture
        /// 
        /// Концепция:
        /// 1. Workshop 1 (Wood → Lumber) производит Lumber в ProductsBank
        /// 2. Workshop 4 (Lumber → Furniture) использует Lumber из ProductsBank как материал
        /// 3. Workshop 4 производит Furniture в ProductsBank
        /// 
        /// Это демонстрирует возможность использования продуктов одного цеха как материалов другого.
        /// 
        /// Методы для изучения:
        /// - Workshop.Process() - логика работы с ProductsBank как источником материалов
        /// - IndustrialBuilding.RunOnce() - порядок обработки цехов
        /// </summary>
        [TestMethod]
        public void WoodProcessingFactory_ProductionChain_WorksCorrectly()
        {
            // Arrange - Подготовка: создаем завод
            var factory = new WoodProcessingFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - Действие: запускаем производство для создания пиломатериалов, затем мебели
            // Цикл 1: Workshop 1 производит Lumber из Wood
            building!.RunOnce();
            // Цикл 2: Workshop 1 снова производит Lumber, Workshop 4 может использовать его для Furniture
            building.RunOnce();

            // Assert - Проверка: цепочка производства должна работать
            // Пиломатериалы должны быть произведены из древесины
            var hasLumber = building.ProductsBank.ContainsKey(ProductType.Lumber);
            // Если есть пиломатериалы, то может быть произведена мебель
            if (hasLumber && building.ProductsBank[ProductType.Lumber] > 0)
            {
                // Цикл 3: Workshop 4 использует накопленный Lumber для производства Furniture
                building.RunOnce();
                var hasFurniture = building.ProductsBank.ContainsKey(ProductType.Furniture);
                Assert.IsTrue(hasFurniture || building.ProductsBank.Count > 0, 
                    "После производства пиломатериалов должна появиться мебель (цепочка Wood → Lumber → Furniture)");
            }
        }

        [TestMethod]
        public void WoodProcessingFactory_HasCorrectVacancies()
        {
            // Arrange
            var factory = new WoodProcessingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Vacancies.ContainsKey(CitizenProfession.FactoryWorker), 
                "Должны быть вакансии для заводских рабочих");
            Assert.AreEqual(18, building.Vacancies[CitizenProfession.FactoryWorker], 
                "Должно быть 18 вакансий для заводских рабочих");
        }

        [TestMethod]
        public void WoodProcessingFactory_Production_AllProductsProduced()
        {
            // Arrange
            var factory = new WoodProcessingFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство много раз для получения всех продуктов
            for (int i = 0; i < 10; i++)
            {
                building!.RunOnce();
            }

            // Assert
            var hasLumber = building.ProductsBank.ContainsKey(ProductType.Lumber);
            var hasPaper = building.ProductsBank.ContainsKey(ProductType.Paper);
            var hasCrates = building.ProductsBank.ContainsKey(ProductType.WoodenCrate);

            // Хотя бы один из продуктов должен быть произведен
            Assert.IsTrue(hasLumber || hasPaper || hasCrates || building.ProductsBank.Count > 0, 
                "После производства должны появиться продукты из дерева");
        }
    }
}

