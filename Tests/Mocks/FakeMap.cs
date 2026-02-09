//using Domain.Map;
//using Domain.Common.Base;
//using System.Collections.Generic;

//namespace Tests.Mocks
//{
//    public class FakeMap
//    {
//        private readonly Dictionary<Position, MapObject> _tiles = new();

//        public bool IsAreaInBounds(Placement area) => true;

//        public bool TrySetMapObject(MapObject obj, Placement area)
//        {
//            foreach (var pos in area.GetAllPositions())
//            {
//                _tiles[pos] = obj;
//            }
//            return true;
//        }

//        public bool TryRemoveMapObject(Placement area)
//        {
//            foreach (var pos in area.GetAllPositions())
//            {
//                _tiles.Remove(pos);
//            }
//            return true;
//        }

//        public MapObject this[Position pos] => _tiles.ContainsKey(pos) ? _tiles[pos] : null;
//    }
//}
