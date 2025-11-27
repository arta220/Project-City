using Domain.Base;
using Domain.Buildings;
using Domain.Enums;
using Domain.Map;

namespace Domain.Factories
{
    public class AllFactories : IMapObjectFactory
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

    public class ShopFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new CommercialBuilding(
                floors: 1,
                maxOccupancy: 50,
                area: new Area(3, 2)
            );
    }

    public class OfficeFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new CommercialBuilding(
                floors: 3,
                maxOccupancy: 50,
                area: new Area(3, 3)
            );
    }

    public class FactoryBuildingFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new IndustrialBuilding(
                floors: 1,
                maxOccupancy: 50,
                area: new Area(5, 5)
            );
    }

    public class WarehouseFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new IndustrialBuilding(
                floors: 1,
                maxOccupancy: 10,
                area: new Area(4, 6)
            );
    }

    public class UrbanParkFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new Park(
                area: new Area(3, 3),
                type: ParkType.UrbanPark
            );
    }
    public class RoadFactory : IRoadFactory
    {
        public MapObject Create() =>
            new Road(
                area: new Area(1, 1)
            );
    }

}
