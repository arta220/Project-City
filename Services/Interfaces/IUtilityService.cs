using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;
using Domain.Enums;


// Smirnov
namespace Services.Interfaces
{
    public interface IUtilityService
    {
        void SimulateUtilitiesBreakdown(int currentTick, List<Building> buildings);
        void FixUtility(Building building, UtilityType utilityType);
        Dictionary<UtilityType, int> GetBrokenUtilities(Building building);
    }
}
