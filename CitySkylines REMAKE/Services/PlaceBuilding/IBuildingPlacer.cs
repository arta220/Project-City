using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitySkylines_REMAKE.Models.Map;
using Core.Models.Base;

namespace CitySkylines_REMAKE.Services.PlaceBuilding
{
    public interface IBuildingPlacer
    {
        public bool TryPlace(MapModel Map, Building Building, int x, int y);
    }
}
