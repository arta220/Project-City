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
    /// Косметический завод
    /// Производит различные виды косметической продукции
    /// </summary>
    public class CosmeticsFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 3,
                maxOccupancy: 120,
                area: new Area(6, 6),
                type: IndustrialBuildingType.Factory
            );

            // 1. Цех производства кремов для кожи
            building.AddWorkshop(
                NaturalResourceType.Chemicals,
                ProductType.SkinCream,
                coeff: 4
            );

            // 2. Цех производства шампуней
            building.AddWorkshop(
                NaturalResourceType.Chemicals,
                ProductType.Shampoo,
                coeff: 5
            );

            // 3. Цех производства парфюмерии
            building.AddWorkshop(
                NaturalResourceType.Chemicals,
                ProductType.Perfume,
                coeff: 3
            );

            // 4. Цех производства декоративной косметики
            building.AddWorkshop(
                NaturalResourceType.Chemicals,
                ProductType.Makeup,
                coeff: 6
            );

            // 5. Цех упаковки косметики в стеклянные флаконы
            building.AddWorkshop(
                NaturalResourceType.Glass,
                ProductType.CosmeticBottle,
                coeff: 4
            );

            // 6. Цех производства средств для ухода за волосами
            building.AddWorkshop(
                NaturalResourceType.Chemicals,
                ProductType.HairCareProduct,
                coeff: 4
            );

            // 7. Цех производства солнцезащитных средств
            building.AddWorkshop(
                NaturalResourceType.Chemicals,
                ProductType.Sunscreen,
                coeff: 3
            );

            // 8. Цех производства наборов для макияжа
            building.AddWorkshop(
                ProductType.Plastic,
                ProductType.MakeupKit,
                coeff: 5
            );

            // 9. Цех производства гигиенических продуктов
            building.AddWorkshop(
                NaturalResourceType.Chemicals,
                ProductType.HygieneProduct,
                coeff: 8
            );

            // 10. Цех производства ароматических свечей
            building.AddWorkshop(
                ProductType.Plastic,
                ProductType.ScentedCandle,
                coeff: 10
            );

            // Инициализация начальных материалов
            building.MaterialsBank[NaturalResourceType.Chemicals] = 500;
            building.MaterialsBank[NaturalResourceType.Water] = 300;
            building.MaterialsBank[NaturalResourceType.Glass] = 200;
            building.MaterialsBank[ProductType.Plastic] = 250;
            building.MaterialsBank[NaturalResourceType.Energy] = 150;

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
    /// Алкогольный завод
    /// Производит различные виды алкогольной продукции
    /// </summary>
    public class AlcoholFactory : IMapObjectFactory
    {
        public MapObject Create()
        {
            var building = new IndustrialBuilding(
                floors: 2,
                maxOccupancy: 80,
                area: new Area(5, 5),
                type: IndustrialBuildingType.Factory
            );

            // 1. Цех производства пива
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Beer,
                coeff: 6
            );

            // 2. Цех производства водки
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Vodka,
                coeff: 4
            );

            // 3. Цех производства вина
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Wine,
                coeff: 5
            );

            // 4. Цех производства виски
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Whiskey,
                coeff: 3
            );

            // 5. Цех производства рома
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Rum,
                coeff: 4
            );

            // 6. Цех производства текилы
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Tequila,
                coeff: 5
            );

            // 7. Цех производства джина
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Gin,
                coeff: 4
            );

            // 8. Цех производства бренди
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Brandy,
                coeff: 3
            );

            // 9. Цех производства шампанского
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Champagne,
                coeff: 5
            );

            // 10. Цех производства ликёров
            building.AddWorkshop(
                NaturalResourceType.Water,
                ProductType.Liqueur,
                coeff: 6
            );

            // Инициализация начальных материалов
            building.MaterialsBank[NaturalResourceType.Water] = 500;
            building.MaterialsBank[NaturalResourceType.Energy] = 200;
            building.MaterialsBank[ProductType.Plastic] = 150;
            building.MaterialsBank[ProductType.GlassJar] = 100;

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
            factory.AddWorkshop(input: ResourceType.PlasticWaste, output: ResourceType.Plastic, coeff: 3);
            factory.MaterialsBank[ResourceType.PlasticWaste] = 100;
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
}