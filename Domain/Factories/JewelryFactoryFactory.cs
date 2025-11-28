using Domain.Base;
using Domain.Buildings;
using Domain.Map;

namespace Domain.Factories
{
    public class JewelryFactoryFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new JewelryFactory(new Area(2, 2));
    }
}
