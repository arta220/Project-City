using Domain.Common.Base.MovingEntities;
using Domain.Common.Enums;
using Domain.Map;

namespace Services.EntityMovement.Profile
{
    public class CitizenProfile : INavigationProfile
    {
        private readonly MapModel _mapModel;
        public CitizenProfile(MapModel mapmodel)
        {
            _mapModel = mapmodel;
        }
        public bool CanEnter(Position pos)
        {
            if (!_mapModel.IsPositionInBounds(pos))
                return false;

            var tile = _mapModel[pos];

            if (tile.MapObject != null)
                return false;

            if (tile.Terrain == TerrainType.Water)
                return false;

            return true;
        }


        public int GetTileCost(Position pos) // Пока без учета всех стоимостей, можно добавить в будущем без поломки системы.
        {
            int baseCost = 10;

            var tile = _mapModel[pos];

            if (tile.Terrain == TerrainType.Plain)
                baseCost = 15;
            else if (tile.Terrain == TerrainType.Mountain)
                baseCost = 30;

            return baseCost;
        }
    }
}
