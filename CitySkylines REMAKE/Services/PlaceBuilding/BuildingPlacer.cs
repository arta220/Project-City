using CitySkylines_REMAKE.Models.Enums;
using CitySkylines_REMAKE.Models.Map;
using Core.Models.Base;
using System.Runtime.InteropServices.Marshalling;
using System.Windows.Data;
using System.Windows.Media.Media3D;



namespace CitySkylines_REMAKE.Services.PlaceBuilding
{
    internal class BuildingPlacer : IBuildingPlacer
    {

        private int width;
        private int height;
        public bool TryPlace(MapModel map, Building building, int x, int y)
        {
            width = building.Width;
            height = building.Height;
            if (!CanPlace(x, y, map))
                return false;
            //GameMap = map;

            OccupyTiles(map, building, x,y);

            return true;
        }
        // булевый метол, проверяет:
        // вмещается ли здание в карту
        // можно ли его расположить на тайлах
        // нет ли других зданий на тайле
        private bool CanPlace(int x, int y, MapModel map)
        {
            //чек, не выходит ли за границы карты
            if (x < 0 || x + width > map.Width ||
                y < 0 || y + height > map.Height)
                return false;

            for (int tileX = x; tileX < x + width; tileX++)
            {
                // y + heith = вниз, приколы нумерации массива
                for (int tileY = y; tileY < y + height; tileY++)
                {
                    var tile = map[tileX, tileY];


                    if (!IsTileSuitableForBuilding(tile))
                        return false;
                    if (map.GetBuildingAt(tileX, tileY) != null)
                        return false;
                }
            }
            return false;
        }

        // булевый метод, проверяет, можно ли расположить на тайле здание
        // можно потом юзать enum для доп проверок
        // взят с предыдущего проекта
        protected bool IsTileSuitableForBuilding(TileModel tile)
        {
            return tile.Terrain != TerrainType.Mountain &&
                   tile.Terrain != TerrainType.Water; // &&
                   //tile.Height <= 0.3f;   я хз зачем эта проверка
        }

        // метод заполняет свойство тайла Building
        // взят с предыдущего проекта
        protected void OccupyTiles(MapModel map, Building building, int x, int y)
        {
            for (int tileX = x; tileX < x + width; tileX++)
            {
                for (int tileY = y; tileY < y + height; tileY++)
                {
                    if (x >= 0 && x < width &&
                        y >= 0 && y < height)
                    {
                        map._buildingsGrid[x, y] = building;
                        map[x, y].Building = building;
                    }
                }
            }
        }
    }


}

