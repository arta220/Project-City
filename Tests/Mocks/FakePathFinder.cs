//using Domain.Map;
//using Services.PathFind;

//public class FakePathFinder : IPathFinder
//{
//    public List<Position> FindPath(Position from, Position to)
//    {
//        var path = new List<Position>();
//        int x = from.X, y = from.Y;
//        while (x != to.X || y != to.Y)
//        {
//            if (x < to.X) x++;
//            else if (x > to.X) x--;
//            if (y < to.Y) y++;
//            else if (y > to.Y) y--;
//            path.Add(new Position(x, y));
//        }
//        return path;
//    }
//}
