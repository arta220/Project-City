using Domain.Common.Base;

namespace Domain.Factories
{
    public interface IMapObjectFactory
    {
        MapObject Create();
    }
    public interface IRoadFactory : IMapObjectFactory { }

}
