using Domain.Base;
using Domain.Buildings;
using Domain.Buildings.Residential;
using Domain.Buildings.Utility;
using Domain.Common.Base;
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
                area: new Area(5, 5)
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
                area: new Area(4, 6)
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
                area: new Area(5, 5)
            );

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
                area: new Area(6, 6)
            );

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
            var factory = new IndustrialBuilding(2, 80, new Area(5, 5));

            factory.AddWorkshop(
                NaturalResourceType.Chemicals,
                ProductType.PharmaceuticalPack,
                coeff: 2
            );

            factory.MaterialsBank[NaturalResourceType.Chemicals] = 100;
            factory.MaterialsBank[NaturalResourceType.Water] = 50;
            factory.MaterialsBank[NaturalResourceType.Energy] = 80;

            return factory;
        }
    }

    /// <summary>
    /// Завод по переработке отходов
    /// </summary>
    public class RecyclingPlantFactoryFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var factory = new IndustrialBuilding(
                floors: 1,
                maxOccupancy: 60,
                area: new Area(4, 4)
            );

            factory.AddWorkshop(
                input: ProductType.Plastic,
                output: ProductType.PlasticBottle,
                coeff: 3
            );

            factory.MaterialsBank[ProductType.Plastic] = 100;
            factory.MaterialsBank[NaturalResourceType.Energy] = 50;
            factory.MaterialsBank[NaturalResourceType.Water] = 30;

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

    /// <summary>
    /// Завод противопожарного оборудования
    /// Производит: огнетушители, пожарные рукава, системы сигнализации, пожарные машины
    /// </summary>
    public class FireEquipmentFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 80,
                area: new Area(6, 6)
            );

            // Цех производства огнетушителей (железо -> огнетушитель)
            building.AddWorkshop(
                NaturalResourceType.Iron,
                ProductType.FireExtinguisher,
                coeff: 3
            );

            // Цех производства пожарных рукавов
            building.AddWorkshop(
                NaturalResourceType.Wood,
                ProductType.FireHose,
                coeff: 2
            );

            // Цех систем пожарной сигнализации (электроника -> система сигнализации)
            building.AddWorkshop(
                ProductType.Electronics,
                ProductType.FireAlarmSystem,
                coeff: 5
            );

            // Цех сборки пожарных машин (железо -> пожарная машина)
            building.AddWorkshop(
                NaturalResourceType.Iron,
                ProductType.FireTruck,
                coeff: 15
            );

            // Инициализация начальных материалов
            building.MaterialsBank[NaturalResourceType.Iron] = 800;
            building.MaterialsBank[NaturalResourceType.Wood] = 400;
            building.MaterialsBank[ProductType.Electronics] = 200;
            building.MaterialsBank[NaturalResourceType.Energy] = 600;
            building.MaterialsBank[NaturalResourceType.Water] = 300;

            return building;
        }
    }

    /// <summary>
    /// Завод промышленных роботов
    /// Производит: промышленных роботов, роботизированные руки, контроллеры, системы автоматизации
    /// </summary>
    public class RoboticsFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 3,
                maxOccupancy: 65,
                area: new Area(7, 7)
            );

            // Цех контроллеров роботов (электроника -> контроллер)
            building.AddWorkshop(
                ProductType.Electronics,
                ProductType.RobotController,
                coeff: 8
            );

            // Цех роботизированных рук (железо -> роботизированная рука)
            building.AddWorkshop(
                NaturalResourceType.Iron,
                ProductType.RobotArm,
                coeff: 12
            );

            // Цех сборки промышленных роботов (контроллер -> промышленный робот)
            building.AddWorkshop(
                ProductType.RobotController,
                ProductType.IndustrialRobot,
                coeff: 2
            );

            // Цех систем автоматизации (контроллер -> система автоматизации)
            building.AddWorkshop(
                ProductType.RobotController,
                ProductType.AutomationSystem,
                coeff: 3
            );

            // Инициализация начальных материалов
            building.MaterialsBank[ProductType.Electronics] = 400;
            building.MaterialsBank[NaturalResourceType.Iron] = 600;
            building.MaterialsBank[NaturalResourceType.Energy] = 800;
            building.MaterialsBank[NaturalResourceType.Water] = 200;

            return building;
        }
    }
}