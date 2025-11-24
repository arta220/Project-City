using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;

namespace Domain.Map
{
    public class PedestrianPath : MapObject
    {
        public bool HasBenches { get; set; }
        public bool HasLighting { get; set; }

        public PedestrianPath(Area area) : base(area)
        {
            HasBenches = true;
            HasLighting = true;
        }
    }
}