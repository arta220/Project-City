//using Domain.Buildings.Residential;
//using Domain.Common.Enums;
//using Domain.Common.Time;
//using Services.Utilities;
//using System.Collections.Generic;

//namespace Tests.Mocks
//{
//    public class FakeUtilityService : IUtilityService
//    {
//        private readonly Dictionary<ResidentialBuilding, HashSet<UtilityType>> _broken = new();

//        public void BreakUtilityForTesting(ResidentialBuilding building, UtilityType utilityType, int currentTick)
//        {
//            if (!_broken.ContainsKey(building))
//                _broken[building] = new HashSet<UtilityType>();
//            _broken[building].Add(utilityType);
//        }

//        public void FixAllUtilities(ResidentialBuilding building) => _broken.Remove(building);

//        public void FixUtility(ResidentialBuilding building, UtilityType utilityType)
//        {
//            if (_broken.ContainsKey(building))
//                _broken[building].Remove(utilityType);
//        }

//        public Dictionary<UtilityType, int> GetBrokenUtilities(ResidentialBuilding building)
//        {
//            if (!_broken.ContainsKey(building)) return new Dictionary<UtilityType, int>();
//            var dict = new Dictionary<UtilityType, int>();
//            foreach (var ut in _broken[building])
//                dict[ut] = 1;
//            return dict;
//        }

//        public UtilityStatistics GetStatistics() => new UtilityStatistics();

//        public void Update() { }

//        public void Update(SimulationTime time)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
