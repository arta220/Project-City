using Domain.Common.Base.MovingEntities;
using Domain.Map;
using Services.EntityMovement.PathFind;
using System.Collections.Generic;

namespace Tests.Mocks
{
    public class FakePathFinder : IPathFinder
    {
        public IEnumerable<Position>? FindPath(Position from, Position to, INavigationProfile profile)
        {
            var path = new List<Position>();
            int x = from.X, y = from.Y;
            while (x != to.X || y != to.Y)
            {
                if (x < to.X) x++;
                else if (x > to.X) x--;
                if (y < to.Y) y++;
                else if (y > to.Y) y--;
                
                var pos = new Position(x, y);
                if (profile.CanEnter(pos))
                {
                    path.Add(pos);
                }
                else
                {
                    // Если не можем войти, возвращаем null
                    return null;
                }
            }
            return path;
        }
    }
}