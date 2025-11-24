using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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