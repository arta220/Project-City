using Domain.Base;
using Domain.Buildings;
using Domain.Buildings.Residential;
using Domain.Buildings.Utility;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Base;
using Domain.Common.Base.MovingEntities;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Infrastructure;
using Domain.Map;

namespace Domain.Factories
{


    public class SmallHouseFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new ResidentialBuilding(
                floors: 1,
                maxOccupancy: 50,
                area: new Area(2, 2)
            );
    }

    public class ApartmentFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new ResidentialBuilding(
                floors: 5,
                maxOccupancy: 250,
                area: new Area(4, 4)
            );
    }

    public class PharmacyFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Pharmacy(area: new Area(1, 1));
    }

    public class ShopFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Shop(area: new Area(2, 2));
    }

    public class SupermarketFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Supermarket(area: new Area(3, 3));
    }

    public class CafeFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Cafe(area: new Area(2, 2));
    }

    public class RestaurantFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Restaurant(area: new Area(3, 3));
    }

    public class GasStationFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new GasStation(area: new Area(2, 2));
    }

    #region Industrial Buildings
    /// <summary>
    /// Здание завода
    /// Завод (имя и иконка регистрируются в BuildingRegistry)
    /// </summary>
    public class FactoryBuildingFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 1,
                maxOccupancy: 50,
                area: new Area(5, 5),
                type: IndustrialBuildingType.Factory
            );
            
            return building;
        }
}

    /// <summary>
    /// Складское здание
    /// Склад (имя и иконка регистрируются в BuildingRegistry)
    /// </summary>
    public class WarehouseFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 1,
                maxOccupancy: 10,
                area: new Area(4, 6),
                type: IndustrialBuildingType.Warehouse
            );

            // Цех по переработке дерева в бумагу
            building.AddWorkshop(
                NaturalResourceType.Wood,
                ProductType.Paper,
                coeff: 2
            );

            // Цех по переработке дерева в мебель
            building.AddWorkshop(
                NaturalResourceType.Wood,
                ProductType.Furniture,
                coeff: 1
            );

            return building;
        }
    }


    /// <summary>
    /// Фабрика добывающей промышленности.
    /// 
    /// КОНЦЕПЦИЯ:
    /// Добывающий завод - это первичное звено производственной цепочки.
    /// Он добывает природные ресурсы из "ничего" (None) и превращает их в базовые материалы:
    /// - Железо (Iron) - для металлургии, коэффициент добычи: 8
    /// - Дерево (Wood) - для деревообработки, коэффициент добычи: 12 (самый эффективный)
    /// - Уголь (Coal) - для энергетики, коэффициент добычи: 10
    /// 
    /// ПРОИЗВОДСТВЕННЫЙ ПРОЦЕСС:
    /// 1. Завод создается с начальным запасом материала "None" (500 единиц)
    /// 2. При вызове RunOnce() каждый цех берет ProductionCoefficient единиц None из MaterialsBank
    /// 3. Каждый цех производит столько же единиц своего продукта в ProductsBank
    /// 
    /// ПОТОК ДАННЫХ:
    /// ВХОД:  MaterialsBank[None] = 500
    /// ПРОЦЕСС: RunOnce() → Workshop.Process() для каждого цеха
    /// ВЫХОД:  ProductsBank[Iron] = 8, ProductsBank[Wood] = 12, ProductsBank[Coal] = 10
    /// 
    /// ИСПОЛЬЗОВАНИЕ:
    /// Добытые ресурсы используются другими заводами:
    /// - Wood → используется WoodProcessingFactory
    /// - Iron → используется RecyclingFactory для производства стали
    /// - Coal → используется для энергетики
    /// </summary>
    public class ResourceExtractionFactory : IMapObjectFactory
    {
        /// <summary>
        /// Создает добывающий завод с настроенными цехами и начальными материалами.
        /// 
        /// Шаги создания:
        /// 1. Создает IndustrialBuilding с параметрами: 1 этаж, 20 человек, площадь 4x4
        /// 2. Настраивает 15 вакансий для заводских рабочих
        /// 3. Добавляет 3 цеха для добычи ресурсов (Iron, Wood, Coal)
        /// 4. Инициализирует MaterialsBank с 500 единицами материала "None"
        /// 
        /// Возвращает: Настроенный IndustrialBuilding готовый к производству
        /// </summary>
        public MapObject Create()
        {
            // Создаем базовое здание завода: одноэтажное, вместимость 20 человек, площадь 4x4 тайла
            var building = new IndustrialBuilding(
                floors: 1,
                maxOccupancy: 20,
                area: new Area(4, 4),
                type: IndustrialBuildingType.Factory
            );

            // Настраиваем вакансии: завод может нанять 15 заводских рабочих
            building.Vacancies[CitizenProfession.FactoryWorker] = 15;

            // Добавляем цех по добыче железа: None → Iron (коэффициент 8)
            // Берет 8 единиц None, производит 8 единиц Iron
            building.AddWorkshop(NaturalResourceType.None, NaturalResourceType.Iron, coeff: 8);
            
            // Добавляем цех по добыче дерева: None → Wood (коэффициент 12 - самый эффективный)
            // Берет 12 единиц None, производит 12 единиц Wood
            building.AddWorkshop(NaturalResourceType.None, NaturalResourceType.Wood, coeff: 12);
            
            // Добавляем цех по добыче угля: None → Coal (коэффициент 10)
            // Берет 10 единиц None, производит 10 единиц Coal
            building.AddWorkshop(NaturalResourceType.None, NaturalResourceType.Coal, coeff: 10);

            // Инициализируем начальный запас материала "None" для производства
            // Этот материал будет использоваться цехами для добычи ресурсов
            building.MaterialsBank[NaturalResourceType.None] = 500;

            return building;
        }
    }

    /// <summary>
    /// Фабрика деревообрабатывающей промышленности.
    /// 
    /// КОНЦЕПЦИЯ:
    /// Деревообрабатывающий завод - это вторичное звено производственной цепочки.
    /// Он перерабатывает дерево (Wood) в различные продукты:
    /// - Пиломатериалы (Lumber) - базовый продукт из дерева, коэффициент: 6
    /// - Мебель (Furniture) - производится из пиломатериалов (цепочка), коэффициент: 3
    /// - Бумага (Paper) - производится напрямую из дерева, коэффициент: 8
    /// - Деревянные ящики (WoodenCrate) - производится напрямую из дерева, коэффициент: 5
    /// 
    /// ПРОИЗВОДСТВЕННЫЙ ПРОЦЕСС:
    /// 1. Завод создается с начальным запасом дерева (300 единиц) в MaterialsBank
    /// 2. При вызове RunOnce() цехи обрабатываются последовательно
    /// 3. Цехи 1, 2, 3 берут Wood из MaterialsBank и производят продукты в ProductsBank
    /// 4. Цех 4 (Furniture) использует Lumber из ProductsBank как материал (цепочка производства)
    /// 
    /// ПРОИЗВОДСТВЕННАЯ ЦЕПОЧКА:
    /// Wood (300) → [Цех 1: Lumber (коэф. 6)] → Lumber в ProductsBank
    ///             → [Цех 2: Paper (коэф. 8)] → Paper в ProductsBank
    ///             → [Цех 3: WoodenCrate (коэф. 5)] → WoodenCrate в ProductsBank
    /// Lumber (из ProductsBank) → [Цех 4: Furniture (коэф. 3)] → Furniture в ProductsBank
    /// 
    /// ОСОБЕННОСТИ:
    /// - Цепочка производства: Wood → Lumber → Furniture (двухэтапный процесс)
    /// - Параллельное производство: из одного Wood можно сделать Paper, Lumber или WoodenCrate
    /// - Workshop.Process() может брать материалы как из MaterialsBank, так и из ProductsBank
    /// 
    /// ПОТОК ДАННЫХ:
    /// ВХОД:  MaterialsBank[Wood] = 300
    /// ПРОЦЕСС: RunOnce() → Workshop.Process() для каждого цеха
    /// ВЫХОД:  ProductsBank[Lumber/Paper/WoodenCrate/Furniture] - готовые продукты
    /// </summary>
    public class WoodProcessingFactory : IMapObjectFactory
    {
        /// <summary>
        /// Создает деревообрабатывающий завод с настроенными цехами и начальными материалами.
        /// 
        /// Шаги создания:
        /// 1. Создает IndustrialBuilding с параметрами: 2 этажа, 25 человек, площадь 5x5
        /// 2. Настраивает 18 вакансий для заводских рабочих
        /// 3. Добавляет 4 цеха для переработки дерева
        ///    - Цех 1: Wood → Lumber (базовый продукт)
        ///    - Цех 2: Lumber → Furniture (цепочка, использует продукт цеха 1)
        ///    - Цех 3: Wood → Paper (параллельное производство)
        ///    - Цех 4: Wood → WoodenCrate (параллельное производство)
        /// 4. Инициализирует MaterialsBank с 300 единицами дерева
        /// 
        /// Возвращает: Настроенный IndustrialBuilding готовый к производству
        /// </summary>
        public MapObject Create()
        {
            // Создаем базовое здание завода: двухэтажное, вместимость 25 человек, площадь 5x5 тайлов
            var building = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 25,
                area: new Area(5, 5),
                type: IndustrialBuildingType.Factory
            );

            // Настраиваем вакансии: завод может нанять 18 заводских рабочих
            building.Vacancies[CitizenProfession.FactoryWorker] = 18;

            // Цех 1: Переработка дерева в пиломатериалы (Wood → Lumber)
            // Берет 6 единиц Wood из MaterialsBank, производит 6 единиц Lumber в ProductsBank
            // Lumber будет использоваться цехом 4 для производства мебели (цепочка)
            building.AddWorkshop(NaturalResourceType.Wood, ProductType.Lumber, coeff: 6);
            
            // Цех 2: Производство мебели из пиломатериалов (Lumber → Furniture)
            // ВАЖНО: Это цепочка производства - цех берет Lumber из ProductsBank (продукт цеха 1)
            // Берет 3 единицы Lumber из ProductsBank, производит 3 единицы Furniture в ProductsBank
            building.AddWorkshop(ProductType.Lumber, ProductType.Furniture, coeff: 3);
            
            // Цех 3: Производство бумаги из дерева (Wood → Paper)
            // Берет 8 единиц Wood из MaterialsBank, производит 8 единиц Paper в ProductsBank
            // Работает параллельно с цехами 1 и 4 (все используют Wood)
            building.AddWorkshop(NaturalResourceType.Wood, ProductType.Paper, coeff: 8);
            
            // Цех 4: Производство деревянных ящиков из дерева (Wood → WoodenCrate)
            // Берет 5 единиц Wood из MaterialsBank, производит 5 единиц WoodenCrate в ProductsBank
            // Работает параллельно с цехами 1 и 3 (все используют Wood)
            building.AddWorkshop(NaturalResourceType.Wood, ProductType.WoodenCrate, coeff: 5);

            // Инициализируем начальный запас дерева для производства
            // Этот материал будет использоваться цехами 1, 3, 4 для производства продуктов
            building.MaterialsBank[NaturalResourceType.Wood] = 300;

            return building;
        }
    }

    /// <summary>
    /// Фабрика перерабатывающей промышленности.
    /// 
    /// КОНЦЕПЦИЯ:
    /// Перерабатывающий завод - это вторичное звено производственной цепочки.
    /// Он перерабатывает базовые природные ресурсы в более сложные продукты:
    /// - Сталь (Steel) - из железа (Iron), коэффициент: 5
    /// - Пластик (Plastic) - из нефти (Oil), коэффициент: 6
    /// - Топливо (Fuel) - из нефти (Oil), коэффициент: 8
    /// - Пластиковые бутылки (PlasticBottle) - из пластика (цепочка), коэффициент: 10
    /// 
    /// ПРОИЗВОДСТВЕННЫЙ ПРОЦЕСС:
    /// 1. Завод создается с начальными запасами: Iron (400 единиц) и Oil (500 единиц) в MaterialsBank
    /// 2. При вызове RunOnce() цехи обрабатываются последовательно
    /// 3. Цехи 1, 2, 3 берут Iron/Oil из MaterialsBank и производят продукты в ProductsBank
    /// 4. Цех 4 (PlasticBottle) использует Plastic из ProductsBank как материал (цепочка производства)
    /// 
    /// ПРОИЗВОДСТВЕННАЯ ЦЕПОЧКА:
    /// Iron (400) → [Цех 1: Steel (коэф. 5)] → Steel в ProductsBank
    /// 
    /// Oil (500) → [Цех 2: Plastic (коэф. 6)] → Plastic в ProductsBank
    ///            → [Цех 3: Fuel (коэф. 8)] → Fuel в ProductsBank
    /// 
    /// Plastic (из ProductsBank) → [Цех 4: PlasticBottle (коэф. 10)] → PlasticBottle в ProductsBank
    /// 
    /// ОСОБЕННОСТИ:
    /// - Два типа входных материалов: Iron и Oil (разные производственные линии)
    /// - Цепочка производства: Oil → Plastic → PlasticBottle (двухэтапный процесс)
    /// - Параллельное производство: из одного Oil можно сделать Plastic или Fuel
    /// - Workshop.Process() может брать материалы как из MaterialsBank, так и из ProductsBank
    /// 
    /// ПОТОК ДАННЫХ:
    /// ВХОД:  MaterialsBank[Iron] = 400, MaterialsBank[Oil] = 500
    /// ПРОЦЕСС: RunOnce() → Workshop.Process() для каждого цеха
    /// ВЫХОД:  ProductsBank[Steel/Plastic/Fuel/PlasticBottle] - готовые продукты
    /// </summary>
    public class RecyclingFactory : IMapObjectFactory
    {
        /// <summary>
        /// Создает перерабатывающий завод с настроенными цехами и начальными материалами.
        /// 
        /// Шаги создания:
        /// 1. Создает IndustrialBuilding с параметрами: 2 этажа, 30 человек, площадь 6x6
        /// 2. Настраивает 22 вакансии для заводских рабочих
        /// 3. Добавляет 4 цеха для переработки ресурсов
        ///    - Цех 1: Iron → Steel (производство стали)
        ///    - Цех 2: Oil → Plastic (производство пластика)
        ///    - Цех 3: Oil → Fuel (производство топлива, параллельно с цехом 2)
        ///    - Цех 4: Plastic → PlasticBottle (цепочка, использует продукт цеха 2)
        /// 4. Инициализирует MaterialsBank с 400 единицами Iron и 500 единицами Oil
        /// 
        /// Возвращает: Настроенный IndustrialBuilding готовый к производству
        /// </summary>
        public MapObject Create()
        {
            // Создаем базовое здание завода: двухэтажное, вместимость 30 человек, площадь 6x6 тайлов
            var building = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 30,
                area: new Area(6, 6),
                type: IndustrialBuildingType.Factory
            );

            // Настраиваем вакансии: завод может нанять 22 заводских рабочих
            building.Vacancies[CitizenProfession.FactoryWorker] = 22;

            // Цех 1: Производство стали из железа (Iron → Steel)
            // Берет 5 единиц Iron из MaterialsBank, производит 5 единиц Steel в ProductsBank
            building.AddWorkshop(NaturalResourceType.Iron, ProductType.Steel, coeff: 5);
            
            // Цех 2: Производство пластика из нефти (Oil → Plastic)
            // Берет 6 единиц Oil из MaterialsBank, производит 6 единиц Plastic в ProductsBank
            // Plastic будет использоваться цехом 4 для производства бутылок (цепочка)
            building.AddWorkshop(NaturalResourceType.Oil, ProductType.Plastic, coeff: 6);
            
            // Цех 3: Производство топлива из нефти (Oil → Fuel)
            // Берет 8 единиц Oil из MaterialsBank, производит 8 единиц Fuel в ProductsBank
            // Работает параллельно с цехом 2 (оба используют Oil)
            building.AddWorkshop(NaturalResourceType.Oil, ProductType.Fuel, coeff: 8);
            
            // Цех 4: Производство пластиковых бутылок из пластика (Plastic → PlasticBottle)
            // ВАЖНО: Это цепочка производства - цех берет Plastic из ProductsBank (продукт цеха 2)
            // Берет 10 единиц Plastic из ProductsBank, производит 10 единиц PlasticBottle в ProductsBank
            building.AddWorkshop(ProductType.Plastic, ProductType.PlasticBottle, coeff: 10);

            // Инициализируем начальные запасы материалов для производства
            // Iron будет использоваться цехом 1 для производства стали
            building.MaterialsBank[NaturalResourceType.Iron] = 400;
            // Oil будет использоваться цехами 2 и 3 для производства пластика и топлива
            building.MaterialsBank[NaturalResourceType.Oil] = 500;

            return building;
        }
    }
    /// <summary>
    /// Завод по производству картона
    /// Производит различные виды картона и картонных изделий
    /// </summary>
    public class CardboardFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 12,
                area: new Area(5, 5),
                type: IndustrialBuildingType.Factory
            );

            // Установка вакансий для рабочих завода
            building.Vacancies[CitizenProfession.FactoryWorker] = 12;

            // Цех подготовки сырья - производство картонных листов
            building.AddWorkshop(
                NaturalResourceType.WoodChips,
                ProductType.CardboardSheets,
                coeff: 8
            );

            // Цех гофрированного картона
            building.AddWorkshop(
                ProductType.CardboardSheets,
                ProductType.CorrugatedCardboard,
                coeff: 4
            );

            // Цех производства картонных коробок
            building.AddWorkshop(
                ProductType.CorrugatedCardboard,
                ProductType.CardboardBoxes,
                coeff: 3
            );

            // Цех плотного картона из макулатуры
            building.AddWorkshop(
                NaturalResourceType.RecycledPaper,
                ProductType.SolidCardboard,
                coeff: 5
            );

            // Цех защитной упаковки
            building.AddWorkshop(
                ProductType.SolidCardboard,
                ProductType.ProtectivePackaging,
                coeff: 4
            );

            // Цех специальных изделий
            building.AddWorkshop(
                ProductType.SolidCardboard,
                ProductType.CardboardTubes,
                coeff: 6
            );

            // Цех упаковки для яиц
            building.AddWorkshop(
                ProductType.CardboardSheets,
                ProductType.EggPackaging,
                coeff: 8
            );

            // Инициализация начальных материалов
            building.MaterialsBank[NaturalResourceType.WoodChips] = 400;
            building.MaterialsBank[NaturalResourceType.RecycledPaper] = 300;
            building.MaterialsBank[NaturalResourceType.Chemicals] = 150;
            building.MaterialsBank[NaturalResourceType.Water] = 200;
            building.MaterialsBank[NaturalResourceType.Energy] = 100;

            return building;
        }
    }

    /// <summary>
    /// Завод по производству упаковки
    /// Производит различные виды упаковки из разных материалов
    /// </summary>
    public class PackagingFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 15,
                area: new Area(6, 6),
                type: IndustrialBuildingType.Warehouse
            );

            // Установка вакансий для рабочих завода
            building.Vacancies[CitizenProfession.FactoryWorker] = 15;

            // Цех картонной упаковки
            building.AddWorkshop(
                ProductType.CardboardSheets,
                ProductType.CardboardBox,
                coeff: 6
            );

            // Цех транспортных коробок
            building.AddWorkshop(
                ProductType.CardboardBox,
                ProductType.ShippingBox,
                coeff: 4
            );

            // Цех пластиковой упаковки
            building.AddWorkshop(
                ProductType.Plastic,
                ProductType.PlasticBottle,
                coeff: 6
            );

            // Цех пищевых контейнеров
            building.AddWorkshop(
                ProductType.Plastic,
                ProductType.FoodContainer,
                coeff: 4
            );

            // Цех стеклянной упаковки
            building.AddWorkshop(
                NaturalResourceType.Glass,
                ProductType.GlassJar,
                coeff: 6
            );

            // Цех косметических флаконов
            building.AddWorkshop(
                NaturalResourceType.Glass,
                ProductType.CosmeticBottle,
                coeff: 4
            );

            // Цех металлической упаковки
            building.AddWorkshop(
                NaturalResourceType.Aluminium,
                ProductType.AluminiumCan,
                coeff: 6
            );

            // Цех деревянной упаковки
            building.AddWorkshop(
                NaturalResourceType.Wood,
                ProductType.WoodenCrate,
                coeff: 3
            );

            // Цех специальной упаковки
            building.AddWorkshop(
                NaturalResourceType.Wood,
                ProductType.PharmaceuticalPack,
                coeff: 5
            );

            // Цех подарочной упаковки
            building.AddWorkshop(
                ProductType.CardboardBox,
                ProductType.GiftBox,
                coeff: 2
            );

            // Инициализация начальных материалов
            building.MaterialsBank[ProductType.CardboardSheets] = 300;
            building.MaterialsBank[ProductType.Plastic] = 250;
            building.MaterialsBank[NaturalResourceType.Glass] = 200;
            building.MaterialsBank[NaturalResourceType.Aluminium] = 150;
            building.MaterialsBank[NaturalResourceType.Wood] = 100;
            building.MaterialsBank[NaturalResourceType.Ink] = 50;

            return building;
        }
    }

    /// <summary>
    /// Фармацевтический завод
    /// </summary>
    public class PharmaceuticalFactoryFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var factory = new IndustrialBuilding(
                2, 
                80, 
                new Area(5, 5),
                type: IndustrialBuildingType.Warehouse
                );
            factory.AddWorkshop(ResourceType.Chemicals, ResourceType.Medicine, 2);
            factory.MaterialsBank[ResourceType.Chemicals] = 100;
            return factory;
        }
    }

    /// <summary>
    /// Завод по переработке отходов и вторичной переработке
    /// </summary>
    public class RecyclingPlantFactoryFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var factory = new IndustrialBuilding(floors: 1,
                maxOccupancy: 60,
                area: new Area(4, 4),
                type: IndustrialBuildingType.Factory);           
            factory.AddWorkshop(input: ResourceType.PlasticWaste,output: ResourceType.Plastic,coeff: 3);
            factory.MaterialsBank[ResourceType.PlasticWaste] = 100;
            return factory;
        }
    }

    /// <summary>
    /// Добывающий завод
    /// Добывает природные ресурсы из земли (железо, медь, нефть, газ, дерево)
    /// </summary>
    public class MiningFactoryFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var factory = new IndustrialBuilding(
                floors: 1,
                maxOccupancy: 40,
                area: new Area(5, 5),
                type: IndustrialBuildingType.Factory
            );

            // Цех добычи железа
            factory.AddWorkshop(
                NaturalResourceType.Energy, // Требуется энергия для добычи
                NaturalResourceType.Iron,
                coeff: 5
            );

            // Цех добычи меди
            factory.AddWorkshop(
                NaturalResourceType.Energy,
                NaturalResourceType.Copper,
                coeff: 4
            );

            // Цех добычи нефти
            factory.AddWorkshop(
                NaturalResourceType.Energy,
                NaturalResourceType.Oil,
                coeff: 3
            );

            // Цех добычи газа
            factory.AddWorkshop(
                NaturalResourceType.Energy,
                NaturalResourceType.Gas,
                coeff: 4
            );

            // Цех добычи дерева
            factory.AddWorkshop(
                NaturalResourceType.Energy,
                NaturalResourceType.Wood,
                coeff: 6
            );

            // Инициализация начальных материалов
            factory.MaterialsBank[NaturalResourceType.Energy] = 500;

            return factory;
        }
    }

    /// <summary>
    /// Древообрабатывающий завод
    /// Обрабатывает дерево в различные продукты (доски, мебель, бумага)
    /// </summary>
    public class WoodProcessingFactoryFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var factory = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 30,
                area: new Area(4, 4),
                type: IndustrialBuildingType.Factory
            );

            // Цех обработки дерева в доски
            factory.AddWorkshop(
                NaturalResourceType.Wood,
                ProductType.Paper,
                coeff: 3
            );

            // Цех производства мебели
            factory.AddWorkshop(
                NaturalResourceType.Wood,
                ProductType.Furniture,
                coeff: 2
            );

            // Цех производства деревянных ящиков
            factory.AddWorkshop(
                NaturalResourceType.Wood,
                ProductType.WoodenCrate,
                coeff: 4
            );

            // Цех производства древесной щепы
            factory.AddWorkshop(
                NaturalResourceType.Wood,
                NaturalResourceType.WoodChips,
                coeff: 5
            );

            // Инициализация начальных материалов
            factory.MaterialsBank[NaturalResourceType.Wood] = 300;
            factory.MaterialsBank[NaturalResourceType.Energy] = 100;

            return factory;
        }
    }
    #endregion

    public class UrbanParkFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Park(
                area: new Area(3, 3),
                type: ParkType.UrbanPark
            );
    }
    public class SquareParkFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Park(
                area: new Area(2, 3),
                type: ParkType.Square
            );
    }

    }
    public class BotanicalGardenParkFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Park(
                area: new Area(4, 4),
                type: ParkType.BotanicalGarden
            );
    }

    public class PlaygroundParkFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Park(
                area: new Area(1, 1),
                type: ParkType.Playground
            );
    }

    public class RecreationAreaParkFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Park(
                area: new Area(2, 3),
                type: ParkType.RecreationArea
            );
    }

    public class PedestrianPathFactory : IMapObjectFactory
    {
        public MapObject Create() => new PedestrianPath();
    }

    public class BicyclePathFactory : IMapObjectFactory
    {
        public MapObject Create() => new BicyclePath();
    }

    public class RoadFactory : IRoadFactory
        {
            public MapObject Create() =>
                new Road(
                    area: new Area(1, 1)
                );
    }
    public class UtilityOfficeFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new UtilityOffice(area: new Area(2, 1));
    }
    public class AirPortFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Port(
                area: new Area(2, 6),
                type: PortType.AirPort
            );
    }

