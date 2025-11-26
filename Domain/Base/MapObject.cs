using Domain.Map;

namespace Domain.Base
{
    public abstract class MapObject
    {
        public Area Area { get; }

        protected MapObject(Area area)
        {
            Area = area;
        }
    }
}