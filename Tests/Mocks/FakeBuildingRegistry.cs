//using Domain.Buildings.Residential;
//using Domain.Common.Base;
//using Domain.Map;
//using Services.BuildingRegistry;
//using System.Collections.Generic;
//using System.Linq;

//namespace Tests.Mocks
//{
//    public class FakeBuildingRegistry : IBuildingRegistry
//    {
//        private readonly List<MapObject> _buildings = new();

//        public void Add(MapObject building) => _buildings.Add(building);

//        public IEnumerable<T> GetBuildings<T>() => _buildings.OfType<T>();

//        public (Placement? placement, bool found) TryGetPlacement(MapObject building)
//        {
//            var res = _buildings.Contains(building);
//            return (new Placement(0, 0, building.Area), res);
//        }
//    }
//}
