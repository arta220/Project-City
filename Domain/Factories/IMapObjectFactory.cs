using Domain.Base;

namespace Domain.Factories
{
    public interface IMapObjectFactory
    {
        MapObject Create();
    }
    public interface IRoadFactory : IMapObjectFactory { }

}
