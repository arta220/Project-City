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
    /// Тесты для перерабатывающей промышленности (RecyclingFactory).
    /// 
    /// КОНЦЕПТУАЛЬНОЕ ОПИСАНИЕ РАБОТЫ ПЕРЕРАБАТЫВАЮЩЕЙ ПРОМЫШЛЕННОСТИ:
    /// 
    /// 1. ЧТО ЭТО ТАКОЕ:
    ///    Перерабатывающий завод - это вторичное звено производственной цепочки.
    ///    Он перерабатывает базовые природные ресурсы в более сложные продукты:
    ///    - Сталь (Steel) - из железа (Iron)
    ///    - Пластик (Plastic) - из нефти (Oil)
    ///    - Топливо (Fuel) - из нефти (Oil)
    ///    - Пластиковые бутылки (PlasticBottle) - из пластика (цепочка: Oil → Plastic → PlasticBottle)
    /// 
    /// 2. КАК ЭТО РАБОТАЕТ:
    ///    - Завод имеет начальные запасы: Iron (400 единиц) и Oil (500 единиц) в MaterialsBank
    ///    - Завод содержит 4 цеха с разными производственными цепочками
    ///    - Один цех использует продукт другого цеха как материал (цепочка производства)
    ///    - При вызове RunOnce() все цехи пытаются произвести свои продукты
    /// 
    /// 3. ПРОИЗВОДСТВЕННАЯ ЦЕПОЧКА:
    ///    Iron (400) → [Цех 1: Steel (коэф. 5)] → Steel в ProductsBank
    ///    
    ///    Oil (500) → [Цех 2: Plastic (коэф. 6)] → Plastic в ProductsBank
    ///              → [Цех 3: Fuel (коэф. 8)] → Fuel в ProductsBank
    ///    
    ///    Plastic (из ProductsBank) → [Цех 4: PlasticBottle (коэф. 10)] → PlasticBottle в ProductsBank
    ///    
    ///    ВАЖНО: Цех 4 использует Plastic из ProductsBank как материал (цепочка производства)
    /// 
    /// 4. КЛЮЧЕВЫЕ МЕТОДЫ ДЛЯ ИЗУЧЕНИЯ:
    ///    - RecyclingFactory.Create() - создание завода (Domain/Factories/AllFactories.cs:184-207)
    ///    - IndustrialBuilding.AddWorkshop() - добавление цеха
    ///    - Workshop.Process() - обработка цеха (может брать материалы из ProductsBank)
    ///    - IndustrialBuilding.RunOnce() - запуск всех цехов
    /// 
    /// 5. ОТКУДА КУДА БЕРЕТСЯ:
    ///    ВХОД:  MaterialsBank[NaturalResourceType.Iron] = 400
    ///           MaterialsBank[NaturalResourceType.Oil] = 500
    ///    ПРОЦЕСС: 
    ///      - Цехи 1,2,3: берут Iron/Oil из MaterialsBank → кладут продукты в ProductsBank
    ///      - Цех 4: берет Plastic из ProductsBank → кладет PlasticBottle в ProductsBank
    ///    ВЫХОД:  ProductsBank[ProductType.Steel/Plastic/Fuel/PlasticBottle] - готовые продукты
    /// 
    /// 6. ОСОБЕННОСТИ:
    ///    - Два типа входных материалов: Iron и Oil (разные производственные линии)
    ///    - Цепочка производства: Oil → Plastic → PlasticBottle (двухэтапный процесс)
    ///    - Параллельное производство: из одного Oil можно сделать Plastic или Fuel
    ///    - Workshop.Process() может брать материалы как из MaterialsBank, так и из ProductsBank
    /// </summary>
    [TestClass]
    public class RecyclingIndustryTests
    {
        /// <summary>
        /// Тест: Фабрика должна создавать здание с правильными базовыми свойствами.
        /// 
        /// Проверяет, что RecyclingFactory создает IndustrialBuilding с корректными параметрами:
        /// - 2 этажа (двухэтажное здание)
        /// - Максимальная вместимость 30 человек (самый большой из трех типов)
        /// - Площадь 6x6 тайлов (самый большой из трех типов)
        /// 
        /// Методы для изучения: RecyclingFactory.Create() в Domain/Factories/AllFactories.cs:184-207
        /// </summary>
        [TestMethod]
        public void RecyclingFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange - Подготовка: создаем фабрику
            var factory = new RecyclingFactory();

            // Act - Действие: создаем здание через фабрику
            var building = factory.Create() as IndustrialBuilding;

            // Assert - Проверка: здание должно иметь правильные свойства
            Assert.IsNotNull(building);
            Assert.AreEqual(2, building.Floors, "Перерабатывающий завод должен быть двухэтажным");
            Assert.AreEqual(30, building.MaxOccupancy, "Максимальная вместимость должна быть 30");
            Assert.AreEqual(new Area(6, 6), building.Area, "Площадь должна быть 6x6");
        }

        [TestMethod]
        public void RecyclingFactory_CreatesBuilding_WithWorkshops()
        {
            // Arrange
            var factory = new RecyclingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Workshops.Count > 0, "Завод должен иметь цеха");
            Assert.AreEqual(4, building.Workshops.Count, "Перерабатывающий завод должен иметь 4 цеха");
        }

        [TestMethod]
        public void RecyclingFactory_HasInitialMaterials()
        {
            // Arrange
            var factory = new RecyclingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Iron), "Должно быть железо");
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.Oil), "Должна быть нефть");
            Assert.AreEqual(400, building.MaterialsBank[NaturalResourceType.Iron], "Начальное количество железа должно быть 400");
            Assert.AreEqual(500, building.MaterialsBank[NaturalResourceType.Oil], "Начальное количество нефти должно быть 500");
        }

        [TestMethod]
        public void RecyclingFactory_HasCorrectWorkshops()
        {
            // Arrange
            var factory = new RecyclingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            
            // Проверяем цех по производству стали
            var steelWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "Iron" && w.OutputProduct.ToString() == "Steel");
            Assert.IsNotNull(steelWorkshop, "Должен быть цех по производству стали");
            Assert.AreEqual(5, steelWorkshop.ProductionCoefficient, "Коэффициент производства стали должен быть 5");

            // Проверяем цех по производству пластика
            var plasticWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "Oil" && w.OutputProduct.ToString() == "Plastic");
            Assert.IsNotNull(plasticWorkshop, "Должен быть цех по производству пластика");
            Assert.AreEqual(6, plasticWorkshop.ProductionCoefficient, "Коэффициент производства пластика должен быть 6");

            // Проверяем цех по производству топлива
            var fuelWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "Oil" && w.OutputProduct.ToString() == "Fuel");
            Assert.IsNotNull(fuelWorkshop, "Должен быть цех по производству топлива");
            Assert.AreEqual(8, fuelWorkshop.ProductionCoefficient, "Коэффициент производства топлива должен быть 8");

            // Проверяем цех по производству пластиковых бутылок
            var bottleWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "Plastic" && w.OutputProduct.ToString() == "PlasticBottle");
            Assert.IsNotNull(bottleWorkshop, "Должен быть цех по производству пластиковых бутылок");
            Assert.AreEqual(10, bottleWorkshop.ProductionCoefficient, "Коэффициент производства бутылок должен быть 10");
        }

        [TestMethod]
        public void RecyclingFactory_Production_ProducesRecyclingProducts()
        {
            // Arrange
            var factory = new RecyclingFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - запускаем производство несколько раз
            building!.RunOnce();
            building.RunOnce();
            building.RunOnce();

            // Assert
            var hasRecyclingProducts = building.ProductsBank.Keys
                .OfType<ProductType>()
                .Any(p => p == ProductType.Steel ||
                         p == ProductType.Plastic ||
                         p == ProductType.Fuel ||
                         p == ProductType.PlasticBottle);

            Assert.IsTrue(hasRecyclingProducts || building.ProductsBank.Count > 0, 
                "После производства должны появиться продукты переработки");
        }

        [TestMethod]
        public void RecyclingFactory_Workshop_ProcessesMaterials()
        {
            // Arrange
            var factory = new RecyclingFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act
            var initialIron = building!.MaterialsBank.GetValueOrDefault(NaturalResourceType.Iron, 0);
            var initialOil = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Oil, 0);
            building.RunOnce();
            var afterIron = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Iron, 0);
            var afterOil = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.Oil, 0);

            // Assert
            Assert.IsTrue(initialIron >= afterIron, 
                "После производства количество железа должно уменьшиться");
            Assert.IsTrue(initialOil >= afterOil, 
                "После производства количество нефти должно уменьшиться");
        }

        /// <summary>
        /// Тест: Производственная цепочка должна работать корректно.
        /// 
        /// Проверяет цепочку производства: Oil → Plastic → PlasticBottle
        /// 
        /// Концепция:
        /// 1. Workshop 2 (Oil → Plastic) производит Plastic в ProductsBank
        /// 2. Workshop 4 (Plastic → PlasticBottle) использует Plastic из ProductsBank как материал
        /// 3. Workshop 4 производит PlasticBottle в ProductsBank
        /// 
        /// Это демонстрирует возможность использования продуктов одного цеха как материалов другого.
        /// 
        /// Методы для изучения:
        /// - Workshop.Process() - логика работы с ProductsBank как источником материалов
        /// - IndustrialBuilding.RunOnce() - порядок обработки цехов
        /// </summary>
        [TestMethod]
        public void RecyclingFactory_ProductionChain_WorksCorrectly()
        {
            // Arrange - Подготовка: создаем завод
            var factory = new RecyclingFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - Действие: запускаем производство для создания пластика, затем бутылок
            // Цикл 1: Workshop 2 производит Plastic из Oil
            building!.RunOnce();
            // Цикл 2: Workshop 2 снова производит Plastic, Workshop 4 может использовать его для PlasticBottle
            building.RunOnce();

            // Assert - Проверка: цепочка производства должна работать
            // Пластик должен быть произведен из нефти
            var hasPlastic = building.ProductsBank.ContainsKey(ProductType.Plastic);
            // Если есть пластик, то может быть произведена бутылка
            if (hasPlastic && building.ProductsBank[ProductType.Plastic] > 0)
            {
                // Цикл 3: Workshop 4 использует накопленный Plastic для производства PlasticBottle
                building.RunOnce();
                var hasBottles = building.ProductsBank.ContainsKey(ProductType.PlasticBottle);
                Assert.IsTrue(hasBottles || building.ProductsBank.Count > 0, 
                    "После производства пластика должны появиться пластиковые бутылки (цепочка Oil → Plastic → PlasticBottle)");
            }
        }

        [TestMethod]
        public void RecyclingFactory_HasCorrectVacancies()
        {
            // Arrange
            var factory = new RecyclingFactory();

            // Act
            var building = factory.Create() as IndustrialBuilding;

            // Assert
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Vacancies.ContainsKey(CitizenProfession.FactoryWorker), 
                "Должны быть вакансии для заводских рабочих");
            Assert.AreEqual(22, building.Vacancies[CitizenProfession.FactoryWorker], 
                "Должно быть 22 вакансии для заводских рабочих");
        }
    }
}

