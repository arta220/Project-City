using Domain.Buildings.Industrial;
using Domain.Common.Base;
using Domain.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Factories.Factory
{
    public class PharmaceuticalFactoryFactory : IMapObjectFactory
    {
        public MapObject Create() =>
            new PharmaceuticalFactory(new Area(5, 5));
    }
}
