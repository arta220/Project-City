using Domain.Map;
using Domain.Common.Base.MovingEntities;
using Domain.Base;

namespace Services.EntityMovement.Profile
{
    public class TransportProfile : INavigationProfile
    {
        private readonly MapModel _map;
        public TransportProfile(MapModel mapModel)
        {
            _map = mapModel;
        }

        public bool CanEnter(Position pos)
        {
            if (!_map.IsPositionInBounds(pos)) return false;

            var tile = _map[pos];
            // Можно въезжать только на дороги
            return tile.MapObject is Road;
        }

        public int GetTileCost(Position pos)
        {
            if (!_map.IsPositionInBounds(pos)) return int.MaxValue / 2;
            var tile = _map[pos];
            // Дорога — дешёвая для проезда
            if (tile.MapObject is Road) return 5;
            return 1000; // почти недоступно
        }
    }
}
