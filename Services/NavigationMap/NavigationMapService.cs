using Domain.Enums;
using Domain.Map;
using Services.BuildingRegistry;

namespace Services.NavigationMap
{

    public class NavigationMapService : INavigationMap
    {
        private readonly MapModel _map;
        private readonly IBuildingRegistry _buildingRegistry;

        public NavigationMapService(MapModel map, IBuildingRegistry buildingRegistry)
        {
            _map = map;
            _buildingRegistry = buildingRegistry;
        }

        public int GetTileCost(Position p)
        {
            var tile = _map[p];

            int baseCost = 10;

            if (tile.Terrain == TerrainType.Plain)
                baseCost = 15;
            else if (tile.Terrain == TerrainType.Mountain)
                baseCost = 30;

            return baseCost;
        }

        public bool IsWalkable(Position p)
        {
            if (!_map.IsPositionInBounds(p))
                return false;

            var tile = _map[p];
            if (tile.MapObject != null)
                return false;

            if (tile.Terrain == TerrainType.Water)
                return false;

            return true;
        }
    }
}
