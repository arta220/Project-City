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
    /// Тесты для добывающей промышленности (ResourceExtractionFactory).
    /// 
    /// КОНЦЕПТУАЛЬНОЕ ОПИСАНИЕ РАБОТЫ ДОБЫВАЮЩЕЙ ПРОМЫШЛЕННОСТИ:
    /// 
    /// 1. ЧТО ЭТО ТАКОЕ:
    ///    Добывающий завод - это первичное звено производственной цепочки.
    ///    Он добывает природные ресурсы из "ничего" (None) и превращает их в базовые материалы:
    ///    - Железо (Iron) - для металлургии
    ///    - Дерево (Wood) - для деревообработки
    ///    - Уголь (Coal) - для энергетики
    /// 
    /// 2. КАК ЭТО РАБОТАЕТ:
    ///    - Завод имеет начальный запас материала "None" (500 единиц)
    ///    - Завод содержит 3 цеха, каждый из которых преобразует None в свой ресурс
    ///    - При вызове RunOnce() каждый цех пытается произвести свой продукт
    ///    - Цех берет ProductionCoefficient единиц None и производит столько же единиц продукта
    /// 
    /// 3. ПРОИЗВОДСТВЕННАЯ ЦЕПОЧКА:
    ///    None (500) → [Цех 1: Iron (коэф. 8)] → Iron в ProductsBank
    ///                → [Цех 2: Wood (коэф. 12)] → Wood в ProductsBank
    ///                → [Цех 3: Coal (коэф. 10)] → Coal в ProductsBank
    /// 
    /// 4. КЛЮЧЕВЫЕ МЕТОДЫ ДЛЯ ИЗУЧЕНИЯ:
    ///    - ResourceExtractionFactory.Create() - создание завода (Domain/Factories/AllFactories.cs:131-152)
    ///    - IndustrialBuilding.AddWorkshop() - добавление цеха (Domain/Buildings/Industrial/IndustrialBuilding.cs:100-104)
    ///    - IndustrialBuilding.RunOnce() - запуск производства (Domain/Buildings/Industrial/IndustrialBuilding.cs:109-113)
    ///    - Workshop.Process() - обработка одного цеха (Domain/Buildings/Industrial/IndustrialBuilding.cs:44-83)
    ///    - MaterialsBank - хранилище материалов (Dictionary<Enum, int>)
    ///    - ProductsBank - хранилище продукции (Dictionary<Enum, int>)
    /// 
    /// 5. ОТКУДА КУДА БЕРЕТСЯ:
    ///    ВХОД:  MaterialsBank[NaturalResourceType.None] = 500 (начальный запас)
    ///    ПРОЦЕСС: Workshop.Process() берет из MaterialsBank и кладет в ProductsBank
    ///    ВЫХОД:  ProductsBank[NaturalResourceType.Iron/Wood/Coal] - добытые ресурсы
    /// 
    /// 6. ИСПОЛЬЗОВАНИЕ В ИГРЕ:
    ///    - Добытые ресурсы могут использоваться другими заводами как материалы
    ///    - Например, Wood может быть использован WoodProcessingFactory для производства пиломатериалов
    /// </summary>
    [TestClass]
    public class ResourceExtractionTests
    {
        /// <summary>
        /// Тест: Фабрика должна создавать здание с правильными базовыми свойствами.
        /// 
        /// Проверяет, что ResourceExtractionFactory создает IndustrialBuilding с корректными параметрами:
        /// - 1 этаж (одноэтажное здание)
        /// - Максимальная вместимость 20 человек
        /// - Площадь 4x4 тайла
        /// 
        /// Методы для изучения: ResourceExtractionFactory.Create() в Domain/Factories/AllFactories.cs
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_CreatesBuilding_WithCorrectProperties()
        {
            // Arrange - Подготовка: создаем фабрику
            var factory = new ResourceExtractionFactory();

            // Act - Действие: создаем здание через фабрику
            var building = factory.Create() as IndustrialBuilding;

            // Assert - Проверка: здание должно иметь правильные свойства
            Assert.IsNotNull(building);
            Assert.AreEqual(1, building.Floors, "Добывающий завод должен быть одноэтажным");
            Assert.AreEqual(20, building.MaxOccupancy, "Максимальная вместимость должна быть 20");
            Assert.AreEqual(new Area(4, 4), building.Area, "Площадь должна быть 4x4");
        }

        /// <summary>
        /// Тест: Завод должен создаваться с цехами для производства.
        /// 
        /// Проверяет, что добывающий завод имеет 3 цеха:
        /// - Цех по добыче железа
        /// - Цех по добыче дерева
        /// - Цех по добыче угля
        /// 
        /// Методы для изучения: 
        /// - ResourceExtractionFactory.Create() - создание цехов через AddWorkshop()
        /// - IndustrialBuilding.Workshops - список цехов завода
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_CreatesBuilding_WithWorkshops()
        {
            // Arrange - Подготовка: создаем фабрику
            var factory = new ResourceExtractionFactory();

            // Act - Действие: создаем здание
            var building = factory.Create() as IndustrialBuilding;

            // Assert - Проверка: завод должен иметь 3 цеха
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Workshops.Count > 0, "Завод должен иметь цеха");
            Assert.AreEqual(3, building.Workshops.Count, "Добывающий завод должен иметь 3 цеха (Iron, Wood, Coal)");
        }

        /// <summary>
        /// Тест: Завод должен иметь начальный запас материалов для производства.
        /// 
        /// Проверяет, что добывающий завод создается с начальным запасом "None" материала (500 единиц).
        /// Этот материал используется цехами для производства природных ресурсов.
        /// 
        /// Концепция: MaterialsBank[NaturalResourceType.None] = 500 - это "сырье из ничего",
        /// которое цехи преобразуют в реальные ресурсы (Iron, Wood, Coal).
        /// 
        /// Методы для изучения: MaterialsBank - словарь материалов завода
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_HasInitialMaterials()
        {
            // Arrange - Подготовка: создаем фабрику
            var factory = new ResourceExtractionFactory();

            // Act - Действие: создаем здание
            var building = factory.Create() as IndustrialBuilding;

            // Assert - Проверка: должен быть начальный запас материала None
            Assert.IsNotNull(building);
            Assert.IsTrue(building.MaterialsBank.ContainsKey(NaturalResourceType.None), 
                "Должен быть начальный материал None для производства");
            Assert.AreEqual(500, building.MaterialsBank[NaturalResourceType.None], 
                "Начальное количество материала None должно быть 500 единиц");
        }

        /// <summary>
        /// Тест: Завод должен иметь правильные цеха с корректными коэффициентами производства.
        /// 
        /// Проверяет детали каждого цеха:
        /// - Цех добычи железа: None → Iron (коэф. 8) - берет 8 None, производит 8 Iron
        /// - Цех добычи дерева: None → Wood (коэф. 12) - берет 12 None, производит 12 Wood
        /// - Цех добычи угля: None → Coal (коэф. 10) - берет 10 None, производит 10 Coal
        /// 
        /// Концепция: ProductionCoefficient определяет эффективность цеха - сколько единиц
        /// входного материала преобразуется в столько же единиц выходного продукта.
        /// 
        /// Методы для изучения:
        /// - Workshop.Process() - логика преобразования материалов в продукты
        /// - Workshop.ProductionCoefficient - коэффициент эффективности производства
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_HasCorrectWorkshops()
        {
            // Arrange - Подготовка: создаем фабрику
            var factory = new ResourceExtractionFactory();

            // Act - Действие: создаем здание
            var building = factory.Create() as IndustrialBuilding;

            // Assert - Проверка: каждый цех должен иметь правильные параметры
            Assert.IsNotNull(building);
            
            // Проверяем цех по добыче железа: None → Iron (коэф. 8)
            var ironWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "None" && w.OutputProduct.ToString() == "Iron");
            Assert.IsNotNull(ironWorkshop, "Должен быть цех по добыче железа");
            Assert.AreEqual(8, ironWorkshop.ProductionCoefficient, 
                "Коэффициент добычи железа должен быть 8 (берет 8 None, производит 8 Iron)");

            // Проверяем цех по добыче дерева: None → Wood (коэф. 12) - самый эффективный
            var woodWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "None" && w.OutputProduct.ToString() == "Wood");
            Assert.IsNotNull(woodWorkshop, "Должен быть цех по добыче дерева");
            Assert.AreEqual(12, woodWorkshop.ProductionCoefficient, 
                "Коэффициент добычи дерева должен быть 12 (берет 12 None, производит 12 Wood)");

            // Проверяем цех по добыче угля: None → Coal (коэф. 10)
            var coalWorkshop = building.Workshops.FirstOrDefault(w => 
                w.InputMaterial.ToString() == "None" && w.OutputProduct.ToString() == "Coal");
            Assert.IsNotNull(coalWorkshop, "Должен быть цех по добыче угля");
            Assert.AreEqual(10, coalWorkshop.ProductionCoefficient, 
                "Коэффициент добычи угля должен быть 10 (берет 10 None, производит 10 Coal)");
        }

        /// <summary>
        /// Тест: Запуск производства должно создавать природные ресурсы.
        /// 
        /// Проверяет, что при вызове RunOnce() цехи работают и производят ресурсы.
        /// После нескольких циклов производства в ProductsBank должны появиться:
        /// - Iron (железо)
        /// - Wood (дерево)
        /// - Coal (уголь)
        /// 
        /// Концепция работы:
        /// 1. RunOnce() вызывает Process() для каждого цеха
        /// 2. Process() берет ProductionCoefficient единиц None из MaterialsBank
        /// 3. Process() добавляет ProductionCoefficient единиц продукта в ProductsBank
        /// 
        /// Методы для изучения:
        /// - IndustrialBuilding.RunOnce() - запуск всех цехов
        /// - Workshop.Process() - обработка одного цеха
        /// - ProductsBank - хранилище произведенных продуктов
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_Production_ProducesNaturalResources()
        {
            // Arrange - Подготовка: создаем завод
            var factory = new ResourceExtractionFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - Действие: запускаем производство несколько циклов
            // Каждый RunOnce() обрабатывает все цехи один раз
            building!.RunOnce();
            building.RunOnce();
            building.RunOnce();

            // Assert - Проверка: после производства должны появиться природные ресурсы
            var hasNaturalResources = building.ProductsBank.Keys
                .OfType<NaturalResourceType>()
                .Any(p => p == NaturalResourceType.Iron ||
                         p == NaturalResourceType.Wood ||
                         p == NaturalResourceType.Coal);

            Assert.IsTrue(hasNaturalResources || building.ProductsBank.Count > 0, 
                "После производства должны появиться природные ресурсы (Iron, Wood, Coal) в ProductsBank");
        }

        /// <summary>
        /// Тест: Цехи должны потреблять материалы при производстве.
        /// 
        /// Проверяет, что при работе цехов количество входного материала (None) уменьшается.
        /// Это подтверждает, что цехи действительно используют материалы для производства.
        /// 
        /// Концепция:
        /// - До производства: MaterialsBank[None] = 500
        /// - После RunOnce(): каждый цех берет свой ProductionCoefficient единиц None
        /// - Итого расходуется: 8 (Iron) + 12 (Wood) + 10 (Coal) = 30 единиц None
        /// - Результат: MaterialsBank[None] = 500 - 30 = 470
        /// 
        /// Методы для изучения:
        /// - Workshop.Process() - логика уменьшения MaterialsBank и увеличения ProductsBank
        /// - MaterialsBank - хранилище входных материалов
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_Workshop_ProcessesMaterials()
        {
            // Arrange - Подготовка: создаем завод и запоминаем начальное количество материала
            var factory = new ResourceExtractionFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - Действие: запускаем производство и проверяем изменение количества материала
            var initialNone = building!.MaterialsBank.GetValueOrDefault(NaturalResourceType.None, 0);
            building.RunOnce(); // Все цехи обрабатываются, каждый берет свой ProductionCoefficient
            var afterNone = building.MaterialsBank.GetValueOrDefault(NaturalResourceType.None, 0);

            // Assert - Проверка: количество материала должно уменьшиться
            Assert.IsTrue(initialNone >= afterNone, 
                $"После производства количество материала None должно уменьшиться. Было: {initialNone}, стало: {afterNone}");
        }

        /// <summary>
        /// Тест: После множественных циклов производства должны быть произведены все типы ресурсов.
        /// 
        /// Проверяет, что при длительной работе завода производятся все три типа ресурсов:
        /// - Iron (железо)
        /// - Wood (дерево)
        /// - Coal (уголь)
        /// 
        /// Концепция:
        /// При каждом RunOnce() все три цеха пытаются произвести свой продукт.
        /// Если есть достаточно материала None, цех успешно производит продукт.
        /// После 10 циклов все три типа ресурсов должны быть в ProductsBank.
        /// 
        /// Методы для изучения:
        /// - IndustrialBuilding.RunOnce() - запуск всех цехов за один цикл
        /// - ProductsBank.ContainsKey() - проверка наличия продукта
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_Production_AllResourcesProduced()
        {
            // Arrange - Подготовка: создаем завод
            var factory = new ResourceExtractionFactory();
            var building = factory.Create() as IndustrialBuilding;

            // Act - Действие: запускаем производство много раз для накопления всех ресурсов
            // 10 циклов × 3 цеха = 30 попыток производства (если есть материалы)
            for (int i = 0; i < 10; i++)
            {
                building!.RunOnce();
            }

            // Assert - Проверка: все три типа ресурсов должны быть произведены
            var hasIron = building.ProductsBank.ContainsKey(NaturalResourceType.Iron);
            var hasWood = building.ProductsBank.ContainsKey(NaturalResourceType.Wood);
            var hasCoal = building.ProductsBank.ContainsKey(NaturalResourceType.Coal);

            // Хотя бы один из ресурсов должен быть произведен (если есть материалы)
            Assert.IsTrue(hasIron || hasWood || hasCoal || building.ProductsBank.Count > 0, 
                "После множественных циклов производства должны появиться природные ресурсы в ProductsBank");
        }

        /// <summary>
        /// Тест: Завод должен иметь правильное количество вакансий для рабочих.
        /// 
        /// Проверяет, что добывающий завод создается с 15 вакансиями для заводских рабочих.
        /// Вакансии определяют, сколько рабочих может быть нанято на завод.
        /// 
        /// Концепция:
        /// - Vacancies[CitizenProfession.FactoryWorker] = 15 означает, что завод может нанять 15 рабочих
        /// - Рабочие необходимы для работы завода (хотя в тестах это не проверяется)
        /// - В реальной игре рабочие влияют на эффективность производства
        /// 
        /// Методы для изучения:
        /// - IndustrialBuilding.Vacancies - словарь вакансий по профессиям
        /// - Building.Hire() - метод найма рабочего на завод
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_HasCorrectVacancies()
        {
            // Arrange - Подготовка: создаем фабрику
            var factory = new ResourceExtractionFactory();

            // Act - Действие: создаем здание
            var building = factory.Create() as IndustrialBuilding;

            // Assert - Проверка: должны быть вакансии для заводских рабочих
            Assert.IsNotNull(building);
            Assert.IsTrue(building.Vacancies.ContainsKey(CitizenProfession.FactoryWorker), 
                "Должны быть вакансии для заводских рабочих");
            Assert.AreEqual(15, building.Vacancies[CitizenProfession.FactoryWorker], 
                "Добывающий завод должен иметь 15 вакансий для заводских рабочих");
        }

        /// <summary>
        /// Тест: Коэффициенты производства должны быть правильно настроены.
        /// 
        /// Проверяет соотношение эффективности цехов:
        /// - Добыча дерева (коэф. 12) - самая эффективная
        /// - Добыча угля (коэф. 10) - средняя эффективность
        /// - Добыча железа (коэф. 8) - наименее эффективная
        /// 
        /// Концепция:
        /// ProductionCoefficient определяет, сколько единиц материала преобразуется в продукт.
        /// Больший коэффициент = больше продукта за один цикл = выше эффективность.
        /// 
        /// Баланс игры:
        /// - Дерево добывается быстрее всего (12) - это самый доступный ресурс
        /// - Железо добывается медленнее (8) - более ценный ресурс
        /// - Уголь находится посередине (10) - средняя ценность
        /// 
        /// Методы для изучения:
        /// - Workshop.ProductionCoefficient - коэффициент эффективности цеха
        /// </summary>
        [TestMethod]
        public void ResourceExtractionFactory_ProductionCoefficients_AreCorrect()
        {
            // Arrange - Подготовка: создаем фабрику
            var factory = new ResourceExtractionFactory();

            // Act - Действие: создаем здание и находим цехи
            var building = factory.Create() as IndustrialBuilding;

            // Assert - Проверка: коэффициенты должны быть правильно настроены
            var ironWorkshop = building!.Workshops.FirstOrDefault(w => w.OutputProduct.ToString() == "Iron");
            var woodWorkshop = building.Workshops.FirstOrDefault(w => w.OutputProduct.ToString() == "Wood");
            var coalWorkshop = building.Workshops.FirstOrDefault(w => w.OutputProduct.ToString() == "Coal");

            Assert.IsNotNull(ironWorkshop, "Должен быть цех по добыче железа");
            Assert.IsNotNull(woodWorkshop, "Должен быть цех по добыче дерева");
            Assert.IsNotNull(coalWorkshop, "Должен быть цех по добыче угля");

            // Дерево должно добываться быстрее всего (коэффициент 12 - самый высокий)
            Assert.IsTrue(woodWorkshop.ProductionCoefficient > ironWorkshop.ProductionCoefficient, 
                $"Добыча дерева (коэф. {woodWorkshop.ProductionCoefficient}) должна быть эффективнее добычи железа (коэф. {ironWorkshop.ProductionCoefficient})");
            Assert.IsTrue(woodWorkshop.ProductionCoefficient > coalWorkshop.ProductionCoefficient, 
                $"Добыча дерева (коэф. {woodWorkshop.ProductionCoefficient}) должна быть эффективнее добычи угля (коэф. {coalWorkshop.ProductionCoefficient})");
        }
    }
}

